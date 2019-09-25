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
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PlcNext.Common.Tools.SDK
{
    internal interface ICMakeConversation
    {
        JArray GetCodeModelFromServer(VirtualDirectory tempDirectory,
                                      VirtualDirectory sourceDirectory,
                                      VirtualDirectory binaryDirectory);
    }

    internal class CMakeConversationExecuter : ICMakeConversation
    {
        private readonly ExecutionContext executionContext;
        private readonly IProcessManager processManager;
        private readonly IBinariesLocator binariesLocator;
        private readonly IEnvironmentService environmentService;
        private readonly IOutputFormatterPool formatterPool;

        public CMakeConversationExecuter(ExecutionContext executionContext, IProcessManager processManager, IBinariesLocator binariesLocator, IEnvironmentService environmentService, IOutputFormatterPool formatterPool)
        {
            this.executionContext = executionContext;
            this.processManager = processManager;
            this.binariesLocator = binariesLocator;
            this.environmentService = environmentService;
            this.formatterPool = formatterPool;
        }

        public JArray GetCodeModelFromServer(VirtualDirectory tempDirectory,
                                             VirtualDirectory sourceDirectory,
                                             VirtualDirectory binaryDirectory)
        {
            JArray codeModel = null;
            StartCMakeConversation().ConfigureAwait(false).GetAwaiter().GetResult();
            return codeModel;


            async Task StartCMakeConversation()
            {
                using (CMakeConversation conversation = await CMakeConversation.Start(processManager, binariesLocator, formatterPool,
                                                                                      tempDirectory, environmentService.Platform == OSPlatform.Windows,
                                                                                      executionContext, sourceDirectory, binaryDirectory))
                {
                    codeModel = await conversation.GetCodeModel();
                }
            }
        }
    }
}
