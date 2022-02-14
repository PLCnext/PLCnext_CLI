﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using PlcNext.CppParser.IncludeManager;

namespace PlcNext.CppParser.CppRipper.CodeModel.Includes
{
    internal class CppIncludeManager :IIncludeManager
    {
        private readonly IFileParser fileParser;
        private readonly IIncludeCache includeCache;
        private readonly ILog log;
        private readonly IFileSystem fileSystem;
        private readonly IEnvironmentService environmentService;

        private const string IncludeFilePath = "{ApplicationData}/{ApplicationName}/include-cache.json";

        private CppCodeModel codeModel;

        public CppIncludeManager(IFileParser fileParser, IIncludeCache includeCache, ILog log, IFileSystem fileSystem, IEnvironmentService environmentService)
        {
            this.fileParser = fileParser;
            this.includeCache = includeCache;
            this.log = log;
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
        }

        public IEnumerable<CodeSpecificException> InitializeCodeModel(CppCodeModel codeModel, IncludeManagerParameter parameter)
        {
            HashSet<string> parsedIncludes = new HashSet<string>(parameter.KnownIncludes);
            List<CodeSpecificException> codeSpecificExceptions = new List<CodeSpecificException>();
            VirtualFile cacheFile = fileSystem.GetFile(IncludeFilePath.CleanPath().ResolvePathName(environmentService.PathNames));
            includeCache.LoadCache(cacheFile);

            UpdateCache(parameter, parsedIncludes, codeModel, codeSpecificExceptions);

            codeModel.IncludeDirectories = parameter.IncludeDirectories;

            codeModel.RegisterIncludeTypeFinder(s => FindIncludeType(s, parameter.IncludeDirectories));
            this.codeModel = codeModel;
            codeModel.AddConstants(includeCache.Entries.SelectMany(e => e.Constants));

            return codeSpecificExceptions;
        }

        private void UpdateCache(IncludeManagerParameter parameter, HashSet<string> parsedIncludes, CppCodeModel codeModel, List<CodeSpecificException> codeSpecificExceptions)
        {
            log.LogVerbose("Start updating the include cache.");
            Stopwatch loadWatch = new Stopwatch();
            loadWatch.Start();
            using (IIncludeCacheTransaction cacheTransaction = includeCache.StartTransaction())
            {
                Stopwatch checkWatch = new Stopwatch();
                checkWatch.Start();
                
                IncludeCacheEntry[] staleEntries = includeCache.Version < JsonIncludeCache.CurrentVersion
                                                       ? includeCache.Entries.ToArray()
                                                       : FindStaleAndOutdatedEntries().ToArray();
                foreach (IncludeCacheEntry staleEntry in staleEntries)
                {
                    cacheTransaction.DeleteEntry(staleEntry);
                }

                checkWatch.Stop();
                log.LogVerbose($"Checked files in {checkWatch.ElapsedMilliseconds} ms.");

                AddOrUpdate(parameter.IncludeDefinitions, cacheTransaction, parsedIncludes, codeModel, codeSpecificExceptions, parameter);
            }

            loadWatch.Stop();
            log.LogVerbose($"Finished updating the include cache in {loadWatch.ElapsedMilliseconds} ms.");

            IEnumerable<IncludeCacheEntry> FindStaleAndOutdatedEntries()
            {
                return includeCache.Entries.Where(e => !fileSystem.FileExists(e.File) || fileSystem.GetLastWriteTime(e.File) != e.LastWriteTime);
            }
        }

        private void AddOrUpdate(IEnumerable<IncludeDefinition> includes, IIncludeCacheTransaction cacheTransaction, HashSet<string> parsedIncludes, CppCodeModel codeModel, List<CodeSpecificException> codeSpecificExceptions, IncludeManagerParameter parameter)
        {
            Stack<IncludeDefinition> unvisited = new Stack<IncludeDefinition>(includes);
            while (unvisited.Any())
            {
                IncludeDefinition include = unvisited.Pop();
                if (TryFindInclude(include, out IncludeFindResult findResult, parsedIncludes, parameter))
                {
                    IEnumerable<IncludeDefinition> childDefinitions;
                    if (findResult.IsInCache)
                    {
                        VirtualFile cacheSourceFile = fileSystem.GetFile(findResult.IncludeCacheEntry.File);
                        VirtualDirectory cacheBaseDirectory = fileSystem.GetDirectory(findResult.IncludeCacheEntry.BaseDirectory);
                        parsedIncludes.Add(cacheSourceFile.GetRelativePath(cacheBaseDirectory));
                        childDefinitions = findResult.IncludeCacheEntry.Includes.Select(i => new IncludeDefinition(i, cacheSourceFile, cacheBaseDirectory));

                        codeModel.AddDefineStatements(findResult.IncludeCacheEntry.DefineStatements);
                    }
                    else
                    {
                        parsedIncludes.Add(findResult.IncludeFile.GetRelativePath(findResult.IncludeBaseDirectory));
                        childDefinitions = ProcessInclude(findResult.IncludeFile, findResult.IncludeBaseDirectory, codeSpecificExceptions, cacheTransaction, codeModel);
                    }

                    foreach (IncludeDefinition childDefinition in childDefinitions)
                    {
                        unvisited.Push(childDefinition);
                    }
                }
            }
        }

        private bool TryFindInclude(IncludeDefinition current, out IncludeFindResult result, HashSet<string> parsedIncludes, IncludeManagerParameter parameter)
        {
            if (current.Include.Trim().StartsWith("<", StringComparison.Ordinal))
            {
                //system includes in form <algorithms> are ignored
                result = null;
                return false;
            }

            //Try parse relative to source
            if (current.DefinitionSourceFile.Parent.TryGetFileFromPath(current.Include, out VirtualFile file))
            {
                if (parsedIncludes.Contains(file.GetRelativePath(current.DefinitionSourceBaseDirectory)))
                {
                    result = null;
                    return false;
                }

                result = includeCache.TryGetCacheEntry(file.FullName, out IncludeCacheEntry cacheEntry)
                             ? new IncludeFindResult(cacheEntry)
                             : new IncludeFindResult(file, current.DefinitionSourceBaseDirectory);
                return true;
            }

            //Parse relative to include path
            foreach (VirtualDirectory includeDirectory in parameter.IncludeDirectories.Select(x => x.Directory).Where(v => v != null))
            {
                if (includeDirectory.TryGetFileFromPath(current.Include, out file))
                {
                    if (parsedIncludes.Contains(file.GetRelativePath(includeDirectory)))
                    {
                        result = null;
                        return false;
                    }

                    result = includeCache.TryGetCacheEntry(file.FullName, out IncludeCacheEntry cacheEntry)
                                 ? new IncludeFindResult(cacheEntry)
                                 : new IncludeFindResult(file, includeDirectory);
                    return true;
                }
            }

            log.LogWarning($"Could not find include {current.Include}. Possible types from these files will not be parsed.");
            result = null;
            return false;
        }

        private IEnumerable<IncludeDefinition> ProcessInclude(VirtualFile findResultIncludeFile, VirtualDirectory findResultIncludeBaseDirectory, List<CodeSpecificException> codeSpecificExceptions, IIncludeCacheTransaction cacheTransaction, CppCodeModel codeModel)
        {
            ParserResult parserResult = fileParser.Parse(findResultIncludeFile);
            if (!parserResult.Success)
            {
                codeSpecificExceptions.AddRange(parserResult.Exceptions);
                cacheTransaction.AddEntry(new IncludeCacheEntry(findResultIncludeFile.FullName, false, findResultIncludeFile.LastWriteTime, findResultIncludeBaseDirectory.FullName, Enumerable.Empty<string>(), Enumerable.Empty<string>(), new Dictionary<string, string>(), Enumerable.Empty<IConstant>()));
                return Enumerable.Empty<IncludeDefinition>();
            }

            foreach (IType type in parserResult.Types.Keys)
            {
                codeModel.AddType(type, findResultIncludeFile, findResultIncludeBaseDirectory);
            }

            codeModel.AddDefineStatements(parserResult.DefineStatements);
            codeModel.AddConstants(parserResult.Constants);

            codeSpecificExceptions.AddRange(parserResult.Exceptions);
            cacheTransaction.AddEntry(new IncludeCacheEntry(findResultIncludeFile.FullName, true, findResultIncludeFile.LastWriteTime, findResultIncludeBaseDirectory.FullName, parserResult.Types.Keys.Select(t => t.FullName), parserResult.Includes, parserResult.DefineStatements, parserResult.Constants));
            return parserResult.Includes.Select(i => new IncludeDefinition(i, findResultIncludeFile, findResultIncludeBaseDirectory));
        }

        private IType FindIncludeType(string fullName, IEnumerable<IncludePath> includeDirectories)
        {
            if (!includeCache.TryGetCacheEntryWithTypeName(fullName, includeDirectories.Select(x => x.Directory)
                                                              .Select(d => d?.FullName ?? string.Empty)
                                                              .ToArray(),
                                                           out IncludeCacheEntry cacheEntry))
            {
                return null;
            }

            log.LogInformation($"Parse type {fullName} from {cacheEntry.File}");
            VirtualFile cacheFile = fileSystem.GetFile(cacheEntry.File);
            VirtualDirectory cacheBaseDirectory = fileSystem.GetDirectory(cacheEntry.BaseDirectory);
            ParserResult parserResult = fileParser.Parse(cacheFile);
            IType result = null;
            foreach (CodeSpecificException codeSpecificException in parserResult.Exceptions)
            {
                log.LogWarning(codeSpecificException.Message);
            }
            if (parserResult.Success)
            {
                foreach (IType type in parserResult.Types.Keys)
                {
                    codeModel?.AddType(type, cacheFile, cacheBaseDirectory);
                    if (type.FullName == fullName)
                    {
                        result = type;
                    }
                }
                codeModel?.AddConstants(parserResult.Constants);
            }

            log.LogInformation(result != null
                                   ? $"Successfully retrieved type {fullName}."
                                   : $"Could not find the type {fullName} in {cacheEntry.File}. This should not happen.");
            return result;

        }
    }
}
