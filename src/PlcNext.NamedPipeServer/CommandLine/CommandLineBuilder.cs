#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.AttributeFilters;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Tools.UI;

namespace PlcNext.NamedPipeServer.CommandLine
{
    internal class CommandLineBuilder : ICommandLineBuilder
    {
        private readonly CommandLineLifetimeScope commandLineContainer;

        public CommandLineBuilder(CommandLineLifetimeScope commandLineContainer)
        {
            this.commandLineContainer = commandLineContainer;
        }

        public IDisposableCommandLineParser BuildCommandLineInstance(IUserInterface userInterface,
                                                                     IProgressVisualizer progressVisualizer,
                                                                     ICommandResultVisualizer commandResultVisualizer,
                                                                     CancellationToken cancellationToken)
        {
            ILifetimeScope lifetimeScope = commandLineContainer.LifetimeScope.BeginLifetimeScope(
                builder =>
                {
                    builder.RegisterInstance(userInterface).As<IUserInterface>().SingleInstance();
                    builder.RegisterInstance(progressVisualizer).As<IProgressVisualizer>().SingleInstance();
                    builder.RegisterInstance(commandResultVisualizer).As<ICommandResultVisualizer>().SingleInstance();
                    builder.Register(_ => cancellationToken).As<CancellationToken>().SingleInstance();
                    Common.BaseDiModule.AddAutoActivatedComponents(builder);
                });
            return new DisposableCommandLineParser(lifetimeScope,lifetimeScope.Resolve<ICommandLineParser>());
        }
        
        private class DisposableCommandLineParser : IDisposableCommandLineParser
        {
            private readonly IDisposable lifetime;
            private readonly ICommandLineParser parser;

            public DisposableCommandLineParser(IDisposable lifetime, ICommandLineParser parser)
            {
                this.lifetime = lifetime;
                this.parser = parser;
            }

            public Task<int> Parse(params string[] args)
            {
                return parser.Parse(args);
            }

            public string GetParseResult(params string[] args)
            {
                return parser.GetParseResult(args);
            }

            public void Dispose()
            {
                lifetime.Dispose();
            }
        }
    }
}