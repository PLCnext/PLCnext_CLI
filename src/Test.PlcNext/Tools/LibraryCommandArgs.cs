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
    public class LibraryCommandArgs
    {
        public string MetaFileDirectory { get; set; }
        public string LibraryLocation { get; set; }
        public string LibraryBuilderLocation { get; set; }
        public string OutputDirectory { get; set; }

        public string LibraryId { get; set; }

        public IEnumerable<string> Targets { get; set; }

        public IEnumerable<string> ExternalLibraries { get; set; }
    }
}
