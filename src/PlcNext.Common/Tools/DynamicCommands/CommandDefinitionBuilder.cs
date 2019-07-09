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
using System.Text;

namespace PlcNext.Common.Tools.DynamicCommands
{
    public class CommandDefinitionBuilder
    {
        private string name = string.Empty;
        private string help = string.Empty;
        private CommandDefinition baseDefinition = null;
        private readonly List<Argument> arguments = new List<Argument>();
        private readonly List<CommandExample> examples = new List<CommandExample>();

        private CommandDefinitionBuilder()
        {
        }

        public static CommandDefinitionBuilder Create()
        {
            return new CommandDefinitionBuilder();
        }

        public CommandDefinitionBuilder SetName(string name)
        {
            this.name = name;
            return this;
        }

        public CommandDefinitionBuilder SetHelp(string help)
        {
            this.help = help;
            return this;
        }

        public CommandDefinitionBuilder AddExample(string command, string description)
        {
            examples.Add(new CommandExample(command, description));
            return this;
        }

        public CommandDefinitionBuilder AddArgument(Argument argument)
        {
            arguments.Add(argument);
            return this;
        }

        public CommandDefinitionBuilder SetBaseDefintion(CommandDefinition baseDefinition)
        {
            this.baseDefinition = baseDefinition;
            return this;
        }

        public ArgumentBuilder CreateArgument()
        {
            return ArgumentBuilder.Create(this);
        }

        public CommandDefinition Build()
        {
            return baseDefinition != null
                       ? new CommandDefinition(name, help, arguments, examples, baseDefinition)
                       : new CommandDefinition(name, help, arguments, examples);
        }
    }
}
