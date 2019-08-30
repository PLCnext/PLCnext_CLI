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
    public class ArgumentBuilder
    {
        private readonly CommandDefinitionBuilder commandDefinitionBuilder;
        private string name = string.Empty;
        private char shortName = default(char);
        private string help = string.Empty;
        private bool mandatory = false;
        private Func<string, (bool success, string message, string newValue)> restriction = null;
        private ArgumentType argumentType = ArgumentType.Bool;
        private object value = null;
        private string setName = null;
        private char separator = Constants.OptionsSeparator;

        private ArgumentBuilder(CommandDefinitionBuilder commandDefinitionBuilder)
        {
            this.commandDefinitionBuilder = commandDefinitionBuilder;
        }

        public static ArgumentBuilder Create(CommandDefinitionBuilder commandDefinitionBuilder)
        {
            return new ArgumentBuilder(commandDefinitionBuilder);
        }

        public ArgumentBuilder SetMandatory(bool mandatory = true)
        {
            this.mandatory = mandatory;
            return this;
        }

        public ArgumentBuilder SetValue(object value)
        {
            this.value = value;
            ArgumentType newType = ArgumentType.SingleValue;
            if (value is bool)
            {
                newType = ArgumentType.Bool;
            }

            if (value.GetType().IsArray)
            {
                newType = ArgumentType.MultipleValue;
            }

            return SetArgumentType(newType);
        }

        public ArgumentBuilder SetName(string name)
        {
            this.name = name;
            return this;
        }

        public ArgumentBuilder SetHelp(string help)
        {
            this.help = help;
            return this;
        }

        public ArgumentBuilder SetShortName(char shortName)
        {
            this.shortName = shortName;
            return this;
        }

        public ArgumentBuilder SetRestriction(Func<string, (bool success, string message, string newValue)> restriction)
        {
            this.restriction = restriction;
            return this;
        }

        public ArgumentBuilder SetArgumentType(ArgumentType argumentType)
        {
            this.argumentType = argumentType;
            return this;
        }

        public ArgumentBuilder SetSetName(string setName)
        {
            this.setName = setName;
            return this;
        }

        public CommandDefinitionBuilder Build()
        {
            Argument argument;
            switch (argumentType)
            {
                case ArgumentType.Bool:
                    argument = new BoolArgument(name, shortName, mandatory, help, setName);
                    break;
                case ArgumentType.SingleValue:
                    argument = new SingleValueArgument(name, shortName, mandatory, restriction, help, setName);
                    break;
                case ArgumentType.MultipleValue:
                    argument = new MultipleValueArgument(name, shortName, mandatory, restriction, help, setName, separator);
                    break;
                default:
                    throw new ArgumentException("argumentType");
            }

            if (value != null)
            {
                argument.SetValue(value);
            }
            return commandDefinitionBuilder.AddArgument(argument);
        }

        public ArgumentBuilder SetSeparator(char separator)
        {
            this.separator = separator;
            return this;
        }
    }

    public enum ArgumentType
    {
        Bool,
        SingleValue,
        MultipleValue
    }
}
