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
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.Common.Commands
{
    public static class EntityExtensions
    {
        public static bool IsCommand(this Entity entity)
        {
            return entity.HasValue<CommandDefinition>()
                   || entity.HasValue<CommandArgs>();
        }

        public static string GetPathCommandArgument(this Entity entity)
        {
            return entity.Value<CommandDefinition>()
                        ?.Argument<SingleValueArgument>(EntityKeys.PathKey)
                        ?.Value
                   ?? entity.Value<CommandArgs>()
                           ?.PropertyValue<string>(EntityKeys.PathKey)
                   ?? string.Empty;
        }

        public static bool HasPathCommandArgument(this Entity entity)
        {
            return entity.Value<CommandDefinition>()
                        ?.Argument<SingleValueArgument>(EntityKeys.PathKey)
                   != null
                   || entity.Value<CommandArgs>()
                           ?.HasPropertyValue(EntityKeys.PathKey, typeof(string))
                   == true;
        }

        public static IEnumerable<string> GetSourceDirectoriesCommandArgument(this Entity entity)
        {
            return entity.Value<CommandDefinition>()
                        ?.Argument<MultipleValueArgument>(EntityKeys.SourceDirectoryKey)
                        ?.Values
                   ?? entity.Value<CommandArgs>()
                           ?.PropertyValue<IEnumerable<string>>(EntityKeys.SourceDirectoryKey)
                   ?? Enumerable.Empty<string>();
        }

        public static bool HasSourceDirectoriesCommandArgument(this Entity entity)
        {
            return entity.Value<CommandDefinition>()
                        ?.Argument<MultipleValueArgument>(EntityKeys.SourceDirectoryKey)
                   != null
                   || entity.Value<CommandArgs>()
                           ?.HasPropertyValue(EntityKeys.SourceDirectoryKey, typeof(IEnumerable<string>))
                   == true;
        }
    }
}
