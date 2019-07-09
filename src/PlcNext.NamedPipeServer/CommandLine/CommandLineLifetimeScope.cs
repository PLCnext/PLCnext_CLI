#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac;

namespace PlcNext.NamedPipeServer.CommandLine
{
    public class CommandLineLifetimeScope
    {
        public CommandLineLifetimeScope(ILifetimeScope lifetimeScope)
        {
            LifetimeScope = lifetimeScope;
        }

        public ILifetimeScope LifetimeScope { get; }
    }
}