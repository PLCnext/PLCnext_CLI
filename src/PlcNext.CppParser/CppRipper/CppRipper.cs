#region Copyright
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
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Includes;

namespace PlcNext.CppParser.CppRipper
{
    internal sealed class CppRipper : IParser
    {
        private readonly ILog log;
        private readonly IFileParser fileParser;
        private readonly IIncludeManager includeManager;

        public CppRipper(ILog log, IFileParser fileParser, IIncludeManager includeManager)
        {
            this.log = log;
            this.fileParser = fileParser;
            this.includeManager = includeManager;
        }

        public ICodeModel Parse(ICollection<VirtualDirectory> sourceDirectories, IEnumerable<IncludePath> includeDirectories, out IEnumerable<CodeSpecificException> loggableExceptions)
        {
            log.LogVerbose($"Creating code model for {string.Join(", ", sourceDirectories.Select(d => d.FullName))} " +
                           $"with include paths {string.Join(", ",includeDirectories.Select(x => x.Directory).Where(d => d != null).Select(d => d.FullName))}");
            Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)> classes = new Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)>();
            Dictionary<string, (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory)> enums = new Dictionary<string, (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory)>();
            Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)> structures = new Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)>();
            HashSet<string> parsedIncludes = new HashSet<string>();
            List<IncludeDefinition> unresolvedIncludes = new List<IncludeDefinition>();
            List<CodeSpecificException> exceptions = new List<CodeSpecificException>();
            Dictionary<string, string> defineStatements = new Dictionary<string, string>();
            HashSet<IConstant> parsedConstants = new HashSet<IConstant>();

            Stopwatch parsingStopwatch = new Stopwatch();
            parsingStopwatch.Start();

            foreach ((VirtualFile file, VirtualDirectory directory) in sourceDirectories.SelectMany(d => d.Files("*.hpp", true).Concat(d.Files("*.h$", true)).Select(f => (f, d))))
            {
                if (!TryParseFile(file, directory, exceptions, out string[] includes, out Dictionary<string, string> defines, out IEnumerable<IConstant> constants))
                {
                    continue;
                }

                unresolvedIncludes.AddRange(includes.Select(include => new IncludeDefinition(include, file, directory)));

                foreach (KeyValuePair<string, string> define in defines.Where(define => !defineStatements.ContainsKey(define.Key)))
                {
                    defineStatements.Add(define.Key, define.Value);
                }

                foreach (IConstant constant in constants)
                {
                    parsedConstants.Add(constant);
                }
            }

            CppCodeModel model = new CppCodeModel(classes, structures, enums, defineStatements, parsedConstants);
            
            model.SourceDirectories = sourceDirectories;

            loggableExceptions = includeManager.InitializeCodeModel(model, new IncludeManagerParameter(
                                                                        includeDirectories,
                                                                        parsedIncludes,
                                                                        unresolvedIncludes));

            parsingStopwatch.Stop();
            log.LogVerbose($"Code model created in {parsingStopwatch.ElapsedMilliseconds} ms.");

            if (exceptions.Any())
            {
                if (exceptions.Count > 1)
                {
                    throw new AggregateException(exceptions);
                }

                throw exceptions[0];
            }

            return model;

            bool TryParseFile(VirtualFile file, VirtualDirectory directory,
                              List<CodeSpecificException> codeSpecificExceptions, out string[] includes,
                              out Dictionary<string, string> defines,
                              out IEnumerable<IConstant> constants)
            {
                ParserResult result = fileParser.Parse(file);
                if (!result.Success)
                {
                    codeSpecificExceptions.AddRange(result.Exceptions);
                    includes = null;
                    defines = null;
                    constants = null;
                    return false;
                }

                List<ParserMessage> messages = new List<ParserMessage>();
                foreach (KeyValuePair<IType, CodePosition> type in result.Types)
                {
                    switch (type.Key)
                    {
                        case CppStructure structure:
                            if (!structures.ContainsKey(structure.FullName))
                            {
                                structures.Add(structure.FullName, (structure, file, directory));
                            }
                            break;
                        case CppClass cppClass:
                            if (!classes.ContainsKey(cppClass.FullName))
                            {
                                classes.Add(cppClass.FullName, (cppClass, file, directory));
                            }
                            break;
                        case CppEnum cppEnum:
                            if (!enums.ContainsKey(cppEnum.FullName))
                            {
                                enums.Add(cppEnum.FullName, (cppEnum, file, directory));
                            }
                            break;
                    }
                }

                parsedIncludes.Add(file.GetRelativePath(directory));
                codeSpecificExceptions.AddRange(result.Exceptions);
                codeSpecificExceptions.AddRange(messages.Select(m => m.ToException(file)));
                includes = result.Includes;
                defines = result.DefineStatements;
                constants = result.Constants;
                return true;
            }
        }
    }
}
