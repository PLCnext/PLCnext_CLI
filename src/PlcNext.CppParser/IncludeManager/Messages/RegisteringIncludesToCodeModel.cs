#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;

namespace PlcNext.CppParser.IncludeManager.Messages
{
    internal class RegisteringIncludesToCodeModel : Message
    {
        public RegisteringIncludesToCodeModel(Message predecessorMessage, CppCodeModel codeModel): base(predecessorMessage)
        {
            CodeModel = codeModel;
        }

        public RegisteringIncludesToCodeModel(IEnumerable<Message> predecessorMessages, CppCodeModel codeModel): base(predecessorMessages)
        {
            CodeModel = codeModel;
        }
        
        public CppCodeModel CodeModel { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
