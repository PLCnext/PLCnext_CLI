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

namespace PlcNext.Common.Tools.Priority
{
    public class SubjectIdentifier : IEquatable<SubjectIdentifier>
    {
        private const string NoneIdentifier = "2d2f9653-568f-45ae-95d3-ad22d0460c25";

        public SubjectIdentifier(string identifier)
        {
            Identifier = string.IsNullOrEmpty(identifier)
                             ? NoneIdentifier
                             : identifier;
        }

        public static SubjectIdentifier None { get; } = NoneIdentifier;

        public static bool IsNullOrNone(SubjectIdentifier identifier) => identifier == null || identifier == None;

        public override string ToString()
        {
            return Identifier == NoneIdentifier ? "None" : Identifier;
        }

        public static implicit operator SubjectIdentifier(string identifier) => new SubjectIdentifier(identifier);

        public string Identifier { get; }

        public bool Equals(SubjectIdentifier other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Identifier.Equals(other.Identifier, StringComparison.OrdinalIgnoreCase);
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

            return Equals((SubjectIdentifier) obj);
        }

        public override int GetHashCode()
        {
            return Identifier.ToUpperInvariant().GetHashCode();
        }

        public static bool operator ==(SubjectIdentifier left, SubjectIdentifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SubjectIdentifier left, SubjectIdentifier right)
        {
            return !Equals(left, right);
        }
    }
}
