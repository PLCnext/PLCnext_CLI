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
using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.Common.Commands
{
    public class CommandEntity : EntityBaseDecorator
    {
        private CommandEntity(IEntityBase entityBase) : base(entityBase)
        {
        }

        public static CommandEntity Decorate(IEntityBase commandEntity)
        {
            return Decorate<CommandEntity>(commandEntity) ?? new CommandEntity(commandEntity);
        }

        public static CommandEntity FindUpperCommand(IEntityBase current)
        {
            if (current == null) throw new ArgumentNullException(nameof(current));
            IEntityBase commandEntity = null;
            do
            {
                if (current.Value<CommandDefinition>() != null)
                {
                    commandEntity = current;
                }

                current = current.Owner;
            } while (commandEntity == null && current != null);

            if (commandEntity == null)
            {
                throw new InvalidOperationException("There should always be a command somewhere.");
            }
            return Decorate<CommandEntity>(commandEntity) ?? new CommandEntity(commandEntity);
        }

        public bool IsCommandArgumentSpecified(string argument)
        {
            return Value<CommandDefinition>()?.Argument<Argument>(argument)
                                             ?.IsDefined == true ||
                   (Value<CommandArgs>()?.HasPropertyValue(argument, typeof(string)) == true &&
                    !string.IsNullOrEmpty(Value<CommandArgs>().PropertyValue<string>(argument)))||
                   (Value<CommandArgs>()?.HasPropertyValue(argument, typeof(IEnumerable<string>)) == true &&
                    Value<CommandArgs>().PropertyValue<IEnumerable<string>>(argument).Any());
        }

        public string Output => this[EntityKeys.OutputKey].Value<string>();

        public string CommandName => Value<CommandDefinition>()?.Name ??
                                        Value<CommandArgs>()?.PropertyValue<string>("CommandName") ??
                                            string.Empty;

        public string GetSingleValueArgument(string argument)
        {
            return Value<CommandDefinition>()?.Argument<SingleValueArgument>(argument)?.Value ??
                   Value<CommandArgs>()?.PropertyValue<string>(argument) ??
                   (HasContent(argument)
                       ? this[argument].Value<string>()
                       : string.Empty);
        }

        public IEnumerable<string> GetMultiValueArgument(string argument)
        {
            return Value<CommandDefinition>()?.Argument<MultipleValueArgument>(argument)?.Values ??
                   Value<CommandArgs>()?.PropertyValue<IEnumerable<string>>(argument) ??
                   (HasContent(argument)
                        ? this[argument].Select(t => t.Value<string>())
                        : Enumerable.Empty<string>());
        }

        public bool GetBoolValueArgument(string argument)
        {
            return Value<CommandDefinition>()?.Argument<BoolArgument>(argument)?.Value ??
                   Value<CommandArgs>()?.PropertyValue<bool>(argument) ??
                   (HasContent(argument)
                       ? this[argument].Value<bool>()
                       : false);
        }
    }
}
