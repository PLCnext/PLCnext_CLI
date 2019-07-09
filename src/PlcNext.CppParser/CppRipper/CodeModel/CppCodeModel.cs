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

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppCodeModel : ICodeModel
    {
        private readonly Dictionary<string, (CppClass, VirtualFile, VirtualDirectory)> classes;
        private readonly Dictionary<string, (CppStructure, VirtualFile, VirtualDirectory)> structures;
        private readonly Dictionary<string, (CppEnum, VirtualFile, VirtualDirectory)> enums;

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

        public IStructure Structure(string structureName)
        {
            return structures.TryGetValue(structureName, out (CppStructure structure, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.structure
                       : null;
        }

        public IClass Class(string className)
        {
            return classes.TryGetValue(className, out (CppClass cppClass, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.cppClass
                       : null;
        }

        public IEnum Enum(string className)
        {
            return enums.TryGetValue(className, out (CppEnum cppEnum, VirtualFile _, VirtualDirectory d) tuple)
                       ? tuple.cppEnum
                       : null;
        }

        public IType Type(string typeName)
        {
            return (IType) Structure(typeName) ?? Class(typeName);
        }

        public VirtualDirectory GetBaseDirectory(IType type)
        {
            return classes.TryGetValue(type.FullName, out (CppClass c, VirtualFile _, VirtualDirectory baseDirectory) tuple1)
                       ? tuple1.baseDirectory
                       : structures.TryGetValue(type.FullName, out (CppStructure s, VirtualFile _, VirtualDirectory baseDirectory) tuple2)
                           ? tuple2.baseDirectory
                           : null;
        }
    }
}
