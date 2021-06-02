#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(CheckingCacheEntry))]
    [Produces(typeof(CacheEntryChecked))]
    internal class CacheEntryValidator : Agent
    {
        private readonly IFileSystem fileSystem;
        public CacheEntryValidator(IMessageBoard messageBoard, IFileSystem fileSystem) : base(messageBoard)
        {
            this.fileSystem = fileSystem;
        }

        protected override void ExecuteCore(Message messageData)
        {
            CheckingCacheEntry checkingCacheEntry = messageData.Get<CheckingCacheEntry>();
            CheckResult result = CheckResult.None;
            if (!fileSystem.FileExists(checkingCacheEntry.EntryToCheck.File))
            {
                result = CheckResult.Missing;
            }
            else if (fileSystem.GetLastWriteTime(checkingCacheEntry.EntryToCheck.File) !=
                     checkingCacheEntry.EntryToCheck.LastWriteTime)
            {
                result = CheckResult.Outdated;
            }
            OnMessage(new CacheEntryChecked(messageData, checkingCacheEntry.EntryToCheck, result));
        }
    }
}
