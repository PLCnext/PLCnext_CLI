﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace PlcNext.CppParser.IncludeManager.Messages
{
    public class IncludeFileProcessUnnecessary : Message
    {
        public IncludeFileProcessUnnecessary(Message predecessorMessage): base(predecessorMessage)
        {
        }

        public IncludeFileProcessUnnecessary(IEnumerable<Message> predecessorMessages): base(predecessorMessages)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
