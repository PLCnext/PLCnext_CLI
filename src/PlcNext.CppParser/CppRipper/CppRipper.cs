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
using System.IO;
using System.Linq;
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Templates.Templates;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Includes;

namespace PlcNext.CppParser.CppRipper
{
    internal class CppRipper : IParser
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
            Dictionary<string, (CppEnum, VirtualFile, VirtualDirectory)> enums = new Dictionary<string, (CppEnum, VirtualFile, VirtualDirectory)>();
            Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)> structures = new Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)>();
            HashSet<string> parsedIncludes = new HashSet<string>();
            List<IncludeDefinition> unresolvedIncludes = new List<IncludeDefinition>();
            List<CodeSpecificException> exceptions = new List<CodeSpecificException>();

            Stopwatch parsingStopwatch = new Stopwatch();
            parsingStopwatch.Start();

            foreach ((VirtualFile file, VirtualDirectory directory) in sourceDirectories.SelectMany(d => d.Files("*.hpp", true).Select(f => (f, d))))
            {
                if (!TryParseFile(file, directory, exceptions, out string[] includes))
                {
                    continue;
                }

                foreach (string include in includes)
                {
                    unresolvedIncludes.Add(new IncludeDefinition(include, file, directory));
                }
            }

            CppCodeModel model = new CppCodeModel(classes, structures, enums);

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
                              List<CodeSpecificException> codeSpecificExceptions, out string[] includes)
            {
                ParserResult result = fileParser.Parse(file);
                if (!result.Success)
                {
                    codeSpecificExceptions.AddRange(result.Exceptions);
                    includes = null;
                    return false;
                }

                List<ParserMessage> messages = new List<ParserMessage>();
                foreach (KeyValuePair<IType, CodePosition> type in result.Types)
                {
                    switch (type.Key)
                    {
                        case CppStructure structure:
                            if (structures.ContainsKey(structure.FullName))
                            {
                                messages.Add(new ParserMessage("CPP0002", type.Value.Line, type.Value.Column));
                            }
                            else
                            {
                                structures.Add(structure.FullName, (structure, file, directory));
                            }
                            break;
                        case CppClass cppClass:
                            if (classes.ContainsKey(cppClass.FullName))
                            {
                                messages.Add(new ParserMessage("CPP0002", type.Value.Line, type.Value.Column));
                            }
                            else
                            {
                                classes.Add(cppClass.FullName, (cppClass, file, directory));
                            }
                            break;
                        case CppEnum cppEnum:
                            if (enums.ContainsKey(cppEnum.FullName))
                            {
                                messages.Add(new ParserMessage("CPP0002", type.Value.Line, type.Value.Column));
                            }
                            else
                            {
                                enums.Add(cppEnum.FullName, (cppEnum, file, directory));
                            }
                            break;
                        default:
                            //do nothing
                            break;
                    }
                }

                parsedIncludes.Add(file.GetRelativePath(directory));
                codeSpecificExceptions.AddRange(result.Exceptions);
                codeSpecificExceptions.AddRange(messages.Select(m => m.ToException(file)));
                includes = result.Includes;
                return true;
            }
        }
    }
}
