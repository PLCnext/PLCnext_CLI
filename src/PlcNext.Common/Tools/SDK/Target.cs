#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Text.RegularExpressions;

namespace PlcNext.Common.Tools.SDK
{
    public class Target : IEquatable<Target>
    {
        private static readonly Regex VersionNumber = new Regex(@"\((?<version>\d+\.\d+\.\d+\.\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ShortVersionNumber = new Regex(@"\((?<shortversion>\d+\.\d+\.\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public string Name { get; }

        public string ShortVersion { get; }

        public string Version { get; }

        public string LongVersion { get; }

        public Target(string name, string longVersion, string version, string shortVersion)
        {
            Name = name;
            LongVersion = longVersion;
            Version = version;
            ShortVersion = shortVersion;
        }

        public Target(string name, string longVersion)
        {
            Name = name;
            LongVersion = longVersion;
            Version = GetVersion();
            ShortVersion = GetShortVersion();

            string GetVersion()
            {
                Match match = VersionNumber.Match(longVersion);
                if (match.Success)
                {
                    return match.Groups["version"].Value;
                }
                return longVersion;
            }

            string GetShortVersion()
            {
                Match match = ShortVersionNumber.Match(longVersion);
                if (match.Success)
                {
                    return match.Groups["shortversion"].Value;
                }
                return longVersion;
            }
        }

        public string GetShortFullName()
        {
            return string.IsNullOrEmpty(ShortVersion) ? Name : $"{Name},{ShortVersion}";
        }

        public string GetFullName()
        {
            string result = Name;
            result = string.IsNullOrEmpty(Version) ? result : $"{result},{Version}";
            return result;
        }

        public string GetLongFullName()
        {
            return string.IsNullOrEmpty(LongVersion) ? Name : $"{Name},{LongVersion}";
        }
        public bool Equals(Target other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name && LongVersion == other.LongVersion;
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

            return Equals((Target)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (LongVersion != null ? LongVersion.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Target left, Target right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Target left, Target right)
        {
            return !Equals(left, right);
        }
    }
}