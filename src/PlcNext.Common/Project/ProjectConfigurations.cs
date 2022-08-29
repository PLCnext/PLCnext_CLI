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

        public IEnumerable<string> ExcludedFiles
        {
            get => Value.ExcludedFiles ?? Enumerable.Empty<string>();
            set
            {
                Value.ExcludedFiles = value.ToArray();
            }
        }
    }
}
