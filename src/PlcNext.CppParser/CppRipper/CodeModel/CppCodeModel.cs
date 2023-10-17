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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal sealed class CppCodeModel : ICodeModel
    {
        private readonly Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)> classes;
        private readonly Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)> structures;
        private readonly Dictionary<string, (CppTemplatedStructure, VirtualFile, VirtualDirectory)> templatedStructures =
            new Dictionary<string, (CppTemplatedStructure, VirtualFile, VirtualDirectory)>();
        private readonly Dictionary<string, (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory)> enums;

        private Func<string, IType> findTypeInIncludes;

        private static readonly Regex TemplateNameRegex = new (@"^(?<Name>[^<]*)<(?<Value>.*)>$",
                                                                RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        private static readonly Regex TemplateValuesRegex = new (@"[^,<>]+(?(?=<)[^>]*>)", 
                                                                RegexOptions.Compiled, TimeSpan.FromSeconds(5));

        private readonly ILog log;

        public CppCodeModel(Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)> classes,
                            Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)> structures,
                            Dictionary<string, (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory)> enums,
                            Dictionary<string, string> defineStatements,
                            IEnumerable<IConstant> constants,
                            ILog log)
        {
            this.classes = classes;
            this.structures = structures;
            this.enums = enums;
            DefineStatements = defineStatements;
            Constants = new HashSet<IConstant>(constants);
            this.log = log;
        }

        public IDictionary<IStructure, VirtualFile> Structures =>
            structures.Values.ToDictionary(t => (IStructure) t.Item1, t => t.Item2);
        public IDictionary<IClass, VirtualFile> Classes =>
            classes.Values.ToDictionary(t => (IClass)t.Item1, t => t.Item2);

        public IDictionary<IEnum, VirtualFile> Enums =>
            enums.Values.ToDictionary(t => (IEnum)t.Item1, t => t.Item2);

        public IDictionary<IType, VirtualFile> Types =>
            classes.Values.Select(t => ((IType) t.Item1, t.Item2))
                   .Concat(structures.Values.Select(t => ((IType) t.Item1, t.Item2)))
                   .Concat(templatedStructures.Values.Select(t => ((IType) t.Item1, t.Item2)))
                   .Concat(enums.Values.Select(t => ((IType) t.Item1, t.Item2)))
                   .ToDictionary(t => t.Item1, t => t.Item2);

        public IDictionary<IStructure, VirtualFile> TemplatedStructures =>
            templatedStructures.Values.ToDictionary(t => (IStructure) t.Item1, t => t.Item2);

        public IEnumerable<IncludePath> IncludeDirectories { get; internal set; }
        public Dictionary<string, string> DefineStatements { get; }
        private HashSet<IConstant> Constants { get; }

        public  IEnumerable<VirtualDirectory> SourceDirectories { get; internal set; }

        internal void AddDefineStatements(IEnumerable<KeyValuePair<string, string>> statements)
        {
            foreach (KeyValuePair<string,string> statement in statements.Where(kv => !DefineStatements.ContainsKey(kv.Key)))
            {
                DefineStatements.Add(statement.Key, statement.Value);
            }
        }

        public void AddConstants(IEnumerable<IConstant> constants)
        {
            foreach (IConstant constant in constants)
            {
                Constants.Add(constant);
            }
        }

        IStructure ICodeModel.GetStructure(string structureName)
        {
            CppStructure result = FindTemplatedStruct(structureName);
            if (result != null)
            {
                return result;
            }
            return structures.TryGetValue(structureName, out (CppStructure structure, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.structure
                       : findTypeInIncludes?.Invoke(structureName) as IStructure;
        }

        private IStructure GetStructure(string structureName)
        {
            CppStructure result = FindTemplatedStruct(structureName);
            if (result != null)
            {
                return result;
            }
            return structures.TryGetValue(structureName, out (CppStructure structure, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.structure
                       : null;
        }

        private CppTemplatedStructure FindTemplatedStruct(string structureName)
        {
            try
            {
                Match match = TemplateNameRegex.Match(structureName);
                if (match.Success)
                {
                    templatedStructures.TryGetValue(structureName, out (CppTemplatedStructure templStruct, VirtualFile _, VirtualDirectory d) templatedResult);
                    if (templatedResult.templStruct != null)
                    {
                        return templatedResult.templStruct;
                    }
                    string name = match.Groups["Name"].Value.Trim();
                    structures.TryGetValue(name, out (CppStructure structure, VirtualFile f, VirtualDirectory d) result);

                    if (result.structure?.IsTemplated == true)
                    {
                        CppTemplatedStructure cppTemplatedStructure = ReplaceTemplateArgumentsWithRealDataTypes(result.structure,
                                                                                    match.Groups["Value"].Value, structureName);
                        templatedStructures.Add(structureName, (cppTemplatedStructure, result.f, result.d));
                        return cppTemplatedStructure;
                    }
                }
            }catch(RegexMatchTimeoutException e)
            {
                log.LogError(new RegexTimeoutException(e.Pattern, structureName).Message);
            }
            return null;

            CppTemplatedStructure ReplaceTemplateArgumentsWithRealDataTypes(CppStructure structure, 
                    string templateValues, string templatedStructName)
            {
                MatchCollection matches = null;

                try
                {
                   matches = TemplateValuesRegex.Matches(templateValues);
                }
                catch (RegexMatchTimeoutException e)
                {
                    log.LogError(new RegexTimeoutException(e.Pattern, structureName).Message);
                }

                List<string> dataTypes = new List<string>();

                foreach (Match match in matches.Cast<Match>())
                {
                    if (string.IsNullOrEmpty(match.Value.Trim()))
                        continue;
                    dataTypes.Add(match.Value.Trim());
                }
                string name = templatedStructName.Replace(string.IsNullOrEmpty(structure.Namespace) ? string.Empty : structure.Namespace + "::", string.Empty, StringComparison.Ordinal);

                return structure.CreateTemplatedStructure(name, dataTypes.ToArray());
            }
        }

        IClass ICodeModel.GetClass(string className)
        {
            return classes.TryGetValue(className, out (CppClass cppClass, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.cppClass
                       : findTypeInIncludes?.Invoke(className) as IClass;
        }

        private IClass GetClass(string className)
        {
            return classes.TryGetValue(className, out (CppClass cppClass, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.cppClass
                       : null;
        }
        
        IEnum ICodeModel.GetEnum(string enumName)
        {
            return enums.TryGetValue(enumName, out (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory) tuple)
                       ? tuple.e
                       : findTypeInIncludes?.Invoke(enumName) as IEnum;
        }

        private IEnum GetEnum(string enumName)
        {
            return enums.TryGetValue(enumName, out (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory) tuple)
                       ? tuple.e
                       : null;
        }

        public IType Type(string typeName)
        {
            return GetStructure(typeName) ?? (IType) GetClass(typeName) ?? GetEnum(typeName) ?? findTypeInIncludes?.Invoke(typeName);
        }

        public VirtualDirectory GetBaseDirectory(IType type)
        {
            return classes.TryGetValue(type.FullName, out (CppClass c, VirtualFile _, VirtualDirectory baseDirectory) tuple1)
                       ? tuple1.baseDirectory
                       : templatedStructures.TryGetValue(type.FullName, out (CppTemplatedStructure s, VirtualFile _, VirtualDirectory baseDirectory)tuple2)
                           ?tuple2.baseDirectory
                           : structures.TryGetValue(type.FullName, out (CppStructure s, VirtualFile _, VirtualDirectory baseDirectory) tuple3)
                               ? tuple3.baseDirectory
                               : enums.TryGetValue(type.FullName, out (CppEnum e, VirtualFile _, VirtualDirectory baseDirectory) tuple4)
                                   ? tuple4.baseDirectory
                                   : null;
        }

        internal void RegisterIncludeTypeFinder(Func<string, IType> includeTypeFinder)
        {
            findTypeInIncludes = includeTypeFinder;
        }

        internal void AddType(IType type, VirtualFile file, VirtualDirectory directory)
        {
            switch (type)
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
        
        public IDictionary<IConstant, string> FindAccessibleConstants(IEnumerable<string> accessibleNamespaces)
        {
            return Constants.Select(c => new
                             {
                                 constant = c,
                                 ns = accessibleNamespaces
                                     .OrderByDescending(n => n.Length)
                                     .FirstOrDefault(n => c.Namespace.StartsWith(n, StringComparison.Ordinal))
                             })
                            .Where(a => a.ns != null)
                            .ToDictionary(a => a.constant, a => a.ns);
        }
    }
}
