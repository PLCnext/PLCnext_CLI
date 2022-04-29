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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools.UI;

namespace PlcNext.CommandLine
{
    public abstract class VerbBase
    {
        internal ICommandManager CommandManager { get; set; }
        internal IUserInterface UserInterface { get; set; }
        internal ILifetimeScope LifetimeScope { get; set; }

        [Option("verbose", HelpText = "Enables verbose output.")]
        public bool Verbose { get; set; }

        [Option("quiet", HelpText = "Suppresses all output.")]
        public bool Quiet { get; set; }

        //This flag will be evaluated by the main program.
        //This is necessary because otherwise the parser would say "unrecognized option"
        [Option("no-sdk-exploration", Hidden = true)]
        public bool NoSdkExploration { get; set; }

        public async Task<int> Execute()
        {
            if (CommandManager == null)
            {
                throw new InvalidOperationException("Command manager is not resolved.");
            }

            UserInterface?.SetVerbosity(Verbose);
            UserInterface?.SetQuiet(Quiet);

            return await Execute(CommandManager).ConfigureAwait(false);
        }

        protected T AddDeprecatedInformation<T>(T commandArgs)
        where T : CommandArgs
        {
            if (commandArgs == null)
            {
                throw new ArgumentNullException(nameof(commandArgs));
            }

            DeprecatedVerbAttribute attribute = this.GetType().GetCustomAttribute<DeprecatedVerbAttribute>();
            if (attribute == null)
            {
                return commandArgs;
            }

            commandArgs.Deprecated = true;
            commandArgs.DeprecatedAlternative = attribute.AlternativeVerb;
            return commandArgs;
        }
        protected T AddVerbName<T>(T commandArgs)
            where T : CommandArgs
        {
            if (commandArgs == null)
            {
                throw new ArgumentNullException(nameof(commandArgs));
            }

            VerbAttribute verb = this.GetType().GetCustomAttribute<VerbAttribute>();
            if (verb == null)
            {
                return commandArgs;
            }

            commandArgs.CommandName = verb.Name;

            return commandArgs;
        }

        protected abstract Task<int> Execute(ICommandManager commandManager);
    }
}
