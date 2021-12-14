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
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Parser;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    public class CppConstant : CppCodeEntity, IConstant, IEquatable<CppConstant>
    {
        internal CppConstant(string name, string attributePrefix, string ns, string value, CppDataType dataType,
                             string[] usings) : base(name, attributePrefix)
        {
            Namespace = ns;
            Value = value;
            DataType = dataType;
            Usings = usings;
        }

        public string Namespace { get; }
        public string Value { get; }
        public string FullName => string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}::{Name}";

        public IEnumerable<string> AccessibleNamespaces => Namespace.IterateNamespacePermutations()
                                                                    .Concat(Usings);
        public IDataType DataType { get; }
        private string[] Usings { get; }

        public override string ToString()
        {
            return $"{FullName} = {Value}";
        }

        public bool Equals(CppConstant other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name && Namespace == other.Namespace;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((CppConstant)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
            }
        }

        public static bool operator ==(CppConstant left, CppConstant right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CppConstant left, CppConstant right)
        {
            return !Equals(left, right);
        }
    }
}