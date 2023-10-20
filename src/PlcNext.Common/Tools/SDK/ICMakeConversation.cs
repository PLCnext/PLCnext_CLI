#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Newtonsoft.Json.Linq;
using PlcNext.Common.Tools.FileSystem;
using System.Threading.Tasks;

namespace PlcNext.Common.Tools.SDK
{
    internal interface ICMakeConversation
    {
        Task<JObject> GetCodeModel(string projectName,
            VirtualDirectory binaryDirectory);

        Task<JObject> GetCache(VirtualDirectory binaryDirectory);
    }
}
