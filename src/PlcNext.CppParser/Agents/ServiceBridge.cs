#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Agents.Net;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(CodeModelCreated))]
    [Consumes(typeof(IncludeCacheUpdateParsingErrors))]
    [Consumes(typeof(InitializeMessage))]
    [Consumes(typeof(ExceptionCancellationTokenCreated))]
    [Produces(typeof(AttributePrefixDefined))]
    [Produces(typeof(CodeModelCreationParameters))]
    internal class ServiceBridge : Agent, IParser
    {
        private readonly ISettingsProvider settingsProvider;
        private readonly ManualResetEventSlim initializeEvent = new ManualResetEventSlim(false);
        private readonly MessageCollector<CodeModelCreated, IncludeCacheUpdateParsingErrors, CodeModelCreationParameters> collector = new MessageCollector<CodeModelCreated, IncludeCacheUpdateParsingErrors, CodeModelCreationParameters>();

        private readonly MessageCollector<CodeModelCreationParameters, ExceptionCancellationTokenCreated> cancelCollector = new MessageCollector<CodeModelCreationParameters, ExceptionCancellationTokenCreated>();
        private InitializeMessage initializeMessage;
        private readonly ILog log;
        
        public ServiceBridge(IMessageBoard messageBoard, ISettingsProvider settingsProvider, ILog log) : base(messageBoard)
        {
            this.settingsProvider = settingsProvider;
            this.log = log;
            AddDisposable(initializeEvent);
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (initializeMessage == null &&
                messageData.TryGet(out initializeMessage))
            {
                OnMessage(new AttributePrefixDefined(initializeMessage, settingsProvider.Settings.AttributePrefix));
                initializeEvent.Set();
            }
            else
            {
                collector.TryPush(messageData);
                cancelCollector.TryPush(messageData);
            }
        }

        public ICodeModel Parse(ICollection<VirtualDirectory> sourceDirectories,
                                     IEnumerable<IncludePath> includeDirectories,
                                     out IEnumerable<CodeSpecificException> loggableExceptions)
        {
            initializeEvent.Wait();
            CodeModelCreationParameters parameters =
                new CodeModelCreationParameters(initializeMessage, sourceDirectories, includeDirectories);
            MessageDomain.CreateNewDomainsFor(parameters);
            
            log.LogVerbose("Start creating code model.");
            Stopwatch parsingStopwatch = new Stopwatch();
            parsingStopwatch.Start();

            CancellationToken token = default;
            OnMessage(parameters);
            cancelCollector.PushAndExecute(parameters, set =>
            {
                token = set.Message2.Token;
            });
            CodeModelCreated codeModelCreated = null;
            IEnumerable<CodeSpecificException> exceptions = null;
            bool finished = collector.PushAndExecute(parameters, set =>
            {
                set.MarkAsConsumed(set.Message1);
                set.MarkAsConsumed(set.Message2);
                set.MarkAsConsumed(set.Message3);
                
                codeModelCreated = set.Message1;
                exceptions = set.Message2.ParsingErrors;
            }, token);
                
            MessageDomain.TerminateDomainsOf(parameters);

            if (!finished)
            {
                throw new FormattableException(
                    "There was an error while creating the code model for this project. More details can be found in the log file if the '--verbose' flag was used.");
            }
            
            parsingStopwatch.Stop();
            log.LogVerbose($"Code model created in {parsingStopwatch.ElapsedMilliseconds} ms.");

            loggableExceptions = exceptions;
                
            if (codeModelCreated.Exceptions.Any())
            {
                if (codeModelCreated.Exceptions.Count > 1)
                {
                    throw new AggregateException(codeModelCreated.Exceptions);
                }

                throw codeModelCreated.Exceptions.First();
            }
                
            return codeModelCreated.CodeModel;
        }
    }
}
