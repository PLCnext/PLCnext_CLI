#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.FileSystem;
using System.Collections.Generic;
using System.Linq;

namespace PlcNext.Common.Project
{
    public class ProjectConfigurations
    {
        private readonly VirtualFile configurationFile;

        private ProjectConfiguration Value { get; }

        internal ProjectConfigurations(ProjectConfiguration value, VirtualFile configurationFile)
        {
            this.configurationFile = configurationFile;
            Value = value;
            CheckPreconditions();
        }

        public ProjectConfigurations()
        {
            Value = new ProjectConfiguration();
            CheckPreconditions();
        }

        private void CheckPreconditions()
        {
            if (!string.IsNullOrEmpty(SolutionVersion) 
                && !string.IsNullOrEmpty(EngineerVersion))
            {
                throw new DeployArgumentsException();
            }
        }

        public bool IsPersistent => configurationFile != null;

        public string EngineerVersion
        {
            get => Value.EngineerVersion;
            set
            {
                if (!string.IsNullOrEmpty(Value.SolutionVersion))
                {
                    throw new DeployArgumentsException();
                }
                Value.EngineerVersion = value;
            }
        }

        public string SolutionVersion 
        {
            get => Value.SolutionVersion;
            set
            {
                if (!string.IsNullOrEmpty(Value.EngineerVersion))
                {
                    throw new DeployArgumentsException();
                }
                Value.SolutionVersion = value;
            }
        }

        public string LibraryVersion
        {
            get => Value.LibraryVersion;
            set
            {
                Value.LibraryVersion = value;
            }
        }

        public string LibraryDescription
        {
            get => Value.LibraryDescription;
            set
            {
                Value.LibraryDescription = value;
            }
        }

        public IEnumerable<(string, string)> LibraryInfo
        {
            get 
            { 
                return Value.LibraryInfo != null 
                    ? Value.LibraryInfo.Select(x => (x.name,x.Value))
                    : Enumerable.Empty<(string, string)>();
            }
            set 
            {
                Value.LibraryInfo = value.Select(x => new ProjectConfigurationLibraryInfo { name = x.Item1, Value = x.Item2 }).ToArray();
            }
        }

        public IEnumerable<string> ExcludedFiles
        {
            get => Value.ExcludedFiles ?? Enumerable.Empty<string>();
            set
            {
                Value.ExcludedFiles = value.ToArray();
            }
        }

        #region SigningProperties
        public bool SignRequested
        {
            get => Value.Sign;
            set
            {
                Value.Sign = value;
            }
        }

        public string PKCS12
        {
            get => Value.Pkcs12;
            set
            {
                Value.Pkcs12 = value;
            }
        }

        public string PrivateKey
        {
            get => Value.PrivateKey;
            set
            {
                Value.PrivateKey = value;
            }
        }

        public string PublicKey
        {
            get => Value.PublicKey;
            set
            {
                Value.PublicKey = value;
            }
        }

        public IEnumerable<string> Certificates
        {
            get => Value.Certificates ?? Enumerable.Empty<string>();
            set
            {
                Value.Certificates = value.ToArray();
            }
        }

        public string TimestampConfiguration
        {
            get => Value.TimestampConfiguration;
            set
            {
                Value.TimestampConfiguration = value;
            }
        }

        public bool Timestamp
        {
            get => Value.Timestamp;
            set
            {
                Value.Timestamp = value;
            }
        }

        public bool NoTimestamp
        {
            get => Value.NoTimestamp;
            set
            {
                Value.NoTimestamp = value;
            }
        }
        #endregion
    }
}
