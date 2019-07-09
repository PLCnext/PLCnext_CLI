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
using System.Text;

namespace Test.PlcNext.Tools
{
    public abstract class TypemetaStructure
    {
        protected TypemetaStructure(string typeName)
        {
            TypeName = typeName;
        }

        public string TypeName { get; }
    }

    public class EnumTypemetaStructure : TypemetaStructure
    {
        public EnumTypemetaStructure(string typeName, string baseType, IEnumerable<EnumSymbol> symbols) : base(typeName)
        {
            Symbols = symbols;
            BaseType = baseType;
        }

        public string BaseType { get; }

        public IEnumerable<EnumSymbol> Symbols { get; }
    }

    public class EnumSymbol
    {
        public EnumSymbol(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public int Value { get; }
    }

    public class StructTypemetaStructure : TypemetaStructure
    {
        public StructTypemetaStructure(string typeName, IEnumerable<TypeMember> typeMembers) : base(typeName)
        {
            TypeMembers = typeMembers;
        }

        public IEnumerable<TypeMember> TypeMembers { get; }

    }

    public class TypeMember
    {
        public TypeMember(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public TypeMember(string name, string type, uint multiplicity) : this(name, type)
        {
            Multiplicity = multiplicity;
            MultiplicityUsed = true;
        }

        public TypeMember(string name, string type, uint multiplicity, string attributes) : this(name, type, multiplicity)
        {
            AttributesUsed = true;
            Attributes = attributes;
        }

        public string Name { get; }
        public string Type { get; }

        public uint Multiplicity { get; }

        public bool MultiplicityUsed { get; }

        public string Attributes { get; }
        public bool AttributesUsed { get; }
    }
}
