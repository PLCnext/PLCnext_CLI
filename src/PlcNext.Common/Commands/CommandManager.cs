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
using System.Text;
using System.Threading.Tasks;

namespace PlcNext.Common.Commands
{
    internal class CommandManager : ICommandManager
    {
        private readonly IEnumerable<ICommand> commands;

        public CommandManager(IEnumerable<ICommand> commands)
        {
            this.commands = commands;
        }

        public async Task<int> Execute(CommandArgs commandArgs)
        {
            ICommand command = commands.FirstOrDefault(c => c.CommandArgsType.IsInstanceOfType(commandArgs));
            if (command != null)
            {
                return await command.Execute(commandArgs).ConfigureAwait(false);
            }
            throw new ArgumentException($@"No command registered for command arguments type {commandArgs.GetType()}",nameof(commandArgs));
        }
    }
}
