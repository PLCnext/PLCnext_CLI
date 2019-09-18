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

        public bool IsCommandArgumentSpecified(string argument)
        {
            return Value<CommandDefinition>()?.Argument<Argument>(argument)
                                             ?.IsDefined == true;
        }

        public string Output => this[EntityKeys.OutputKey].Value<string>();

        public string CommandName => Value<CommandDefinition>()?.Name ?? string.Empty;

        public string GetSingleValueArgument(string argument)
        {
            return Value<CommandDefinition>()?.Argument<SingleValueArgument>(argument)?.Value ??
                   (HasContent(argument)
                       ? this[argument].Value<string>()
                       : string.Empty);
        }

        public IEnumerable<string> GetMultiValueArgument(string argument)
        {
            return Value<CommandDefinition>()?.Argument<MultipleValueArgument>(argument)?.Values ??
                   (HasContent(argument)
                        ? this[argument].Select(t => t.Value<string>())
                        : Enumerable.Empty<string>());
        }
    }
}
