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
using System.Runtime.Serialization;
using System.Text;

namespace PlcNext.Common.Tools
{
    public class FormattableException : Exception, IEquatable<FormattableException>
    {
        public FormattableException()
        {
        }

        public FormattableException(string message) : base(message)
        {
        }

        public FormattableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public bool Equals(FormattableException other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Message == other.Message;
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

            return Equals((FormattableException)obj);
        }

        public override int GetHashCode()
        {
            return (Message != null ? Message.GetHashCode(StringComparison.Ordinal) : 0);
        }

        public static bool operator ==(FormattableException left, FormattableException right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FormattableException left, FormattableException right)
        {
            return !Equals(left, right);
        }
    }
}
