#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;

namespace PlcNext.Common.Tools.IO
{
    internal class DirectoryPackService : IDirectoryPackService
    {
        private readonly ExecutionContext executionContext;

        public DirectoryPackService(ExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        public void Pack(VirtualDirectory directory, string destination, ChangeObservable observable = null)
        {
            try
            {
                ZipFile.CreateFromDirectory(directory.FullName, destination);
            }
            catch (Exception e)
            {
                executionContext.WriteError(e.Message + Environment.NewLine + e.StackTrace, false);
                throw new FormattableException($"Directory {directory.FullName} could not be packed as zip file. See log for details.");
            }
        }
    }
}
