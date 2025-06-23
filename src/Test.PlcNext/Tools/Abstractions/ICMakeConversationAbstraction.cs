﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Newtonsoft.Json.Linq;

namespace Test.PlcNext.Tools.Abstractions
{
    internal interface ICMakeConversationAbstraction : IAbstraction
    {
        JObject CodeModel { get; set; }
    }
}
