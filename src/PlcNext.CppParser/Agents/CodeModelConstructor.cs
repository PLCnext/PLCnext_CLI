#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;
using Agents.Net;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.CppRipper;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.IncludeManager;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(SourceDirectoriesParsed))]
    [Produces(typeof(CodeModelCreated))]
    internal class CodeModelConstructor : Agent
    {
        public CodeModelConstructor(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            SourceDirectoriesParsed directoriesParsed = messageData.Get<SourceDirectoriesParsed>();
            Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)> classes = new Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)>();
            Dictionary<string, (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory)> enums = new Dictionary<string, (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory)>();
            Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)> structures = new Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)>();
            List<CodeSpecificException> exceptions = new List<CodeSpecificException>();
            List<IncludeDefinition> includes = new List<IncludeDefinition>();
            List<string> parsedFiles = new List<string>();
            Dictionary<string, string> defineStatements = new Dictionary<string, string>();
            foreach (FileResult result in directoriesParsed.Results)
            {
                exceptions.AddRange(result.Errors.Select(e => e.ToException(result.File)));
                includes.AddRange(result.Includes.Select(i => new IncludeDefinition(i, result.File, result.RootDirectory)));
                parsedFiles.Add(result.File.GetRelativePath(result.RootDirectory));
                foreach (KeyValuePair<string, string> defineStatement in result.DefineStatements.Where(kv => !defineStatements.ContainsKey(kv.Key)))
                {
                    defineStatements.Add(defineStatement.Key, defineStatement.Value);
                }
                
                foreach (KeyValuePair<IType, CodePosition> type in result.Types)
                {
                    AddType(type, result);
                }
            }

            CppCodeModel codeModel = new CppCodeModel(classes, structures, enums, defineStatements)
            {
                SourceDirectories = directoriesParsed.Results.Select(r => r.RootDirectory)
                                                     .Distinct()
            };
            OnMessage(new CodeModelCreated(messageData, codeModel, includes, exceptions, parsedFiles));

            void AddType(KeyValuePair<IType, CodePosition> type, FileResult result)
            {
                switch (type.Key)
                {
                    case CppClass @class:
                        if (classes.ContainsKey(@class.FullName))
                        {
                            exceptions.Add(
                                new ParserMessage("CPP0002", type.Value.Line, type.Value.Column).ToException(result.File));
                        }
                        else
                        {
                            classes.Add(@class.FullName, (@class, result.File, result.RootDirectory));
                        }

                        break;
                    case CppEnum @enum:
                        if (enums.ContainsKey(@enum.FullName))
                        {
                            exceptions.Add(
                                new ParserMessage("CPP0002", type.Value.Line, type.Value.Column).ToException(result.File));
                        }
                        else
                        {
                            enums.Add(@enum.FullName, (@enum, result.File, result.RootDirectory));
                        }

                        break;
                    case CppStructure structure:
                        if (structures.ContainsKey(structure.FullName))
                        {
                            exceptions.Add(
                                new ParserMessage("CPP0002", type.Value.Line, type.Value.Column).ToException(result.File));
                        }
                        else
                        {
                            structures.Add(structure.FullName, (structure, result.File, result.RootDirectory));
                        }

                        break;
                    default:
                        //do nothing
                        break;
                }
            }
        }
    }
}
