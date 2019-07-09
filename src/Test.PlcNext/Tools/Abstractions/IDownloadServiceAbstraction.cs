#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;

namespace Test.PlcNext.Tools.Abstractions
{
    public interface IDownloadServiceAbstraction : IAbstraction
    {
        void RemoveFromServer(string path);
        void ChangeFileContent(string path, Stream resourceStream);
    }
}
