#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Newtonsoft.Json.Linq;
using NSubstitute;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedCMakeConversationAbstraction : ICMakeConversationAbstraction
    {
        private readonly ICMakeConversation cmakeConversation = Substitute.For<ICMakeConversation>();

        private JObject codeModel;

        public JObject CodeModel
        {
            get
            {
                return codeModel ?? new JObject();
            }
            set
            {
                codeModel = value;
            }
        }

        public MockedCMakeConversationAbstraction()
        {
            cmakeConversation.GetCodeModelFromServer(null, null, null)
                    .ReturnsForAnyArgs(info => new JArray(CodeModel));
        }

        public void Dispose()
        {
            
        }

        public void Initialize(InstancesRegistrationSource exportProvider, Action<string> printMessage)
        {
            exportProvider.AddInstance(cmakeConversation);
        }
    }
}
