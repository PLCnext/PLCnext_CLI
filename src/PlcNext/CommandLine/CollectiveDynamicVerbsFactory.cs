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
            if (createdTypes.ContainsKey(path))
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

                foreach (Argument argument in definition.Arguments)
                {
                    OptionValueType valueType;
                    switch (argument)
                    {
                        case BoolArgument _:
                            valueType = OptionValueType.WithoutValue;
                            break;
                        case SingleValueArgument _:
                            valueType = OptionValueType.SingleValue;
                            break;
                        case MultipleValueArgument _:
                            valueType = OptionValueType.MultipleValue;
                            break;
                        default:
                            throw new ArgumentException("Unkown value type.");
                    }
                    builder.AddOption(argument.Name, argument.ShortName, argument.Mandatory, argument.Help, valueType);
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
