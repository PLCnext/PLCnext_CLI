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

namespace PlcNext.Common.Commands
{
    public class GenerateLibraryCommandArgs : CommandArgs
    {
        public GenerateLibraryCommandArgs(string path, string metaFilesDirectory, string libraryLocation,
                                          string outputDirectory, string libraryGuid, IEnumerable<string> targets, IEnumerable<string> externalLibraries,
                                          IEnumerable<string> sources)
        {
            Path = path;
            MetaFilesDirectory = metaFilesDirectory;
            LibraryLocation = libraryLocation;
            OutputDirectory = outputDirectory;
            LibraryGuid = libraryGuid;
            Targets = targets;
            Sources = sources;
            ExternalLibraries = externalLibraries;
        }

        public string Path { get; }
        
        public string MetaFilesDirectory { get; }
        
        public string LibraryLocation { get; }
        
        public string OutputDirectory { get; }

        public string LibraryGuid { get; }

        public IEnumerable<string> Targets { get; }
        public IEnumerable<string> Sources { get; }

        public IEnumerable<string> ExternalLibraries { get; }
    }
}
