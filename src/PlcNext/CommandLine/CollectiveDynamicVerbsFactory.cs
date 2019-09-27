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
using CommandLine;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.CommandLine
{
    internal class CollectiveDynamicVerbsFactory : IDynamicVerbFactory, IDisposable
    {
        private readonly IEnumerable<IDynamicCommandProvider> commandProviders;

        private readonly Dictionary<IEnumerable<string>, Type> createdTypes =
            new Dictionary<IEnumerable<string>, Type>(new SequenceEqualComparer<string>());
        private readonly Dictionary<IEnumerable<string>, IEnumerable<Type>> resultCache =
            new Dictionary<IEnumerable<string>, IEnumerable<Type>>(new SequenceEqualComparer<string>());
        private readonly Dictionary<Type, CommandDefinition> typeDefintionMatch =
            new Dictionary<Type, CommandDefinition>();

        public CollectiveDynamicVerbsFactory(IEnumerable<IDynamicCommandProvider> commandProviders)
        {
            this.commandProviders = commandProviders;
        }

        public IEnumerable<Type> GetDynamicVerbs(IEnumerable<string> path)
        {
            path = path.ToArray();
            if (resultCache.ContainsKey(path))
            {
                return resultCache[path];
            }

            List<CommandDefinition> definitions = new List<CommandDefinition>();
            foreach (IDynamicCommandProvider commandProvider in commandProviders)
            {
                definitions.AddRange(commandProvider.GetCommands(path));
            }

            List<Type> types = new List<Type>();
            Type baseType = null;
            if (createdTypes.ContainsKey(path) &&
                createdTypes[path].GetCustomAttribute<UseChildVerbsAsCategoryAttribute>() == null)
            {
                baseType = createdTypes[path];
            }
            foreach (CommandDefinition definition in definitions)
            {
                VerbTypeFactory builder = VerbTypeFactory.Create(definition.Name, definition.Help);
                if (baseType != null)
                {
                    builder.SetBaseType(baseType);
                }

                if (definition.UseChildVerbsAsCategory)
                {
                    builder.EnableUseChildVerbsAsCategory();
                }

                foreach (Argument argument in definition.Arguments)
                {
                    OptionValueType valueType;
                    char separator = default(char);
                    switch (argument)
                    {
                        case BoolArgument _:
                            valueType = OptionValueType.WithoutValue;
                            break;
                        case SingleValueArgument _:
                            valueType = OptionValueType.SingleValue;
                            break;
                        case MultipleValueArgument arg:
                            valueType = OptionValueType.MultipleValue;
                            separator = arg.Separator;
                            break;
                        default:
                            throw new ArgumentException("Unknown value type.");
                    }
                    if(valueType == OptionValueType.MultipleValue)
                    {
                        builder.AddOption(argument.Name, argument.ShortName, argument.Mandatory, argument.Help, valueType, argument.SetName, separator);
                    }
                    else
                    {
                        builder.AddOption(argument.Name, argument.ShortName, argument.Mandatory, argument.Help, valueType, argument.SetName);
                    }
                }

                foreach (CommandExample commandExample in definition.Examples)
                {
                    builder.AddExample(commandExample.Command, commandExample.Description);
                }
                Type type = builder.Build();
                types.Add(type);
                createdTypes.Add(path.Concat(new[] {definition.Name}).ToArray(),
                                 type);
                typeDefintionMatch.Add(type, definition);
            }
            resultCache.Add(path,types);
            return types;
        }

        public CommandDefinition GetCommandDefintionForVerb(Type dynamicVerb)
        {
            return typeDefintionMatch[dynamicVerb];
        }

        public IEnumerable<Type> GetDynamicVerbs(Type currentVerb)
        {
            IEnumerable<string> path = createdTypes.FirstOrDefault(kv => kv.Value == currentVerb).Key
                                       ?? GetVerbPath();

            IEnumerable<string> GetVerbPath()
            {
                return new[] {currentVerb.GetCustomAttribute<VerbAttribute>().Name};
            }

            return GetDynamicVerbs(path);
        }

        public void Dispose()
        {
            VerbTypeFactory.ClearStaticCaches();
        }

        private class SequenceEqualComparer<T> : IEqualityComparer<IEnumerable<T>>
        {
            public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
            {
                if (x == null)
                {
                    return y == null;
                }

                if (y == null)
                {
                    return false;
                }
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                int hashCode = 13;
                unchecked
                {
                    foreach (T value in obj)
                    {
                        hashCode = (hashCode * 397) ^ value.GetHashCode();
                    }
                }

                return hashCode;
            }
        }
    }
}
