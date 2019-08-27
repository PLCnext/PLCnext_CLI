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

        public CMakeConversationExecuter(ExecutionContext executionContext, IProcessManager processManager, IBinariesLocator binariesLocator, IEnvironmentService environmentService)
        {
            this.executionContext = executionContext;
            this.processManager = processManager;
            this.binariesLocator = binariesLocator;
            this.environmentService = environmentService;
        }

        public JArray GetCodeModelFromServer(VirtualDirectory tempDirectory,
                                             VirtualDirectory sourceDirectory,
                                             VirtualDirectory binaryDirectory)
        {
            JArray codeModel = null;
            StartCMakeConversation().Wait();
            return codeModel;


            async Task StartCMakeConversation()
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        using (CMakeConversation conversation = await CMakeConversation.Start(processManager, binariesLocator,
                                                                                              tempDirectory, environmentService.Platform == OSPlatform.Windows,
                                                                                              executionContext, sourceDirectory, binaryDirectory))
                        {
                            codeModel = await conversation.GetCodeModel();
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        if (!IsTimeout(e))
                        {
                            throw;
                        }
                    }
                }
                throw new TimeoutException();

                bool IsTimeout(Exception exception)
                {
                    return exception is TimeoutException ||
                           exception is AggregateException aggregate &&
                           aggregate.InnerExceptions.Any(e => e is TimeoutException);
                }
            }
        }
    }
}
