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
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppCodeModel : ICodeModel
    {
        private readonly Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)> classes;
        private readonly Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)> structures;
        private readonly Dictionary<string, (CppEnum, VirtualFile, VirtualDirectory)> enums;

        private Func<string, IType> findTypeInIncludes;

        public CppCodeModel(Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)> classes,
                            Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)> structures,
                            Dictionary<string, (CppEnum, VirtualFile, VirtualDirectory)> enums)
        {
            this.classes = classes;
            this.structures = structures;
            this.enums = enums;
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
                   .ToDictionary(t => t.Item1, t => t.Item2);

        public IEnumerable<IncludePath> IncludeDirectories { get; internal set; }

        public  IEnumerable<VirtualDirectory> SourceDirectories { get; internal set; }

        public IStructure GetStructure(string structureName)
        {
            return structures.TryGetValue(structureName, out (CppStructure structure, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.structure
                       : findTypeInIncludes?.Invoke(structureName) as IStructure;
        }

        public IClass GetClass(string className)
        {
            return classes.TryGetValue(className, out (CppClass cppClass, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.cppClass
                       : findTypeInIncludes?.Invoke(className) as IClass;
        }

        public IEnum GetEnum(string enumName)
        {
            return enums.TryGetValue(enumName, out (CppEnum cppEnum, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.cppEnum
                       : findTypeInIncludes?.Invoke(enumName) as IEnum;
        }

        public IType Type(string typeName)
        {
            return GetStructure(typeName) ?? (IType) GetClass(typeName) ?? GetEnum(typeName);
        }

        public VirtualDirectory GetBaseDirectory(IType type)
        {
            return classes.TryGetValue(type.FullName, out (CppClass c, VirtualFile _, VirtualDirectory baseDirectory) tuple1)
                       ? tuple1.baseDirectory
                       : structures.TryGetValue(type.FullName, out (CppStructure s, VirtualFile _, VirtualDirectory baseDirectory) tuple2)
                           ? tuple2.baseDirectory
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
                default:
                    //do nothing
                    break;
            }
        }
    }
}
