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

namespace PlcNext.CppParser.IncludeManager
{
    internal sealed class IncludeCacheEntry : IEquatable<IncludeCacheEntry>
    {
        //For Json.Net
        public IncludeCacheEntry()
        {
            
        }
        
        public IncludeCacheEntry(string file, bool parsedSuccessfully, DateTime lastWriteTime, string baseDirectory,
                                 IEnumerable<string> types, IEnumerable<string> includes,
                                 Dictionary<string, string> defineStatements,
                                 IEnumerable<IConstant> constants)
        {
            File = file;
            ParsedSuccessfully = parsedSuccessfully;
            LastWriteTime = lastWriteTime;
            BaseDirectory = baseDirectory;
            Types = types;
            Includes = includes;
            DefineStatements = defineStatements;
            Constants = constants.Select(IncludeConstant.Parse);
        }

        public bool Equals(IncludeCacheEntry other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return File == other.File && ParsedSuccessfully == other.ParsedSuccessfully && LastWriteTime.Equals(other.LastWriteTime) && BaseDirectory == other.BaseDirectory && Equals(Types, other.Types) && Equals(Includes, other.Includes);
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

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((IncludeCacheEntry) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (File != null ? File.GetHashCode(StringComparison.Ordinal) : 0);
                hashCode = (hashCode * 397) ^ ParsedSuccessfully.GetHashCode();
                hashCode = (hashCode * 397) ^ LastWriteTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (BaseDirectory != null ? BaseDirectory.GetHashCode(StringComparison.Ordinal) : 0);
                hashCode = (hashCode * 397) ^ (Types != null ? Types.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Includes != null ? Includes.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(IncludeCacheEntry left, IncludeCacheEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IncludeCacheEntry left, IncludeCacheEntry right)
        {
            return !Equals(left, right);
        }

        public string File { get; set; }
        public bool ParsedSuccessfully { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string BaseDirectory { get; set; }
        public IEnumerable<string> Types { get; set; }
        public IEnumerable<string> Includes { get; set; }
        public Dictionary<string, string> DefineStatements { get; set; } = new Dictionary<string, string>();

        public IEnumerable<IncludeConstant> Constants { get; set; } = Array.Empty<IncludeConstant>();

        public override string ToString()
        {
            return $"{nameof(File)}: {File}, {nameof(ParsedSuccessfully)}: {ParsedSuccessfully}, {nameof(LastWriteTime)}: {LastWriteTime}, {nameof(BaseDirectory)}: {BaseDirectory}, {nameof(Types)}: {string.Join(", ", Types)}, {nameof(Includes)}: {string.Join(", ",Includes)}, {nameof(DefineStatements)}: {string.Join("; ", DefineStatements.Select(kv => $"{kv.Key}: {kv.Value}"))}";
        }
    }
}
