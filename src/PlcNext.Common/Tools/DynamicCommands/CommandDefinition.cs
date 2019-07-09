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

namespace PlcNext.Common.Tools.DynamicCommands
{
    public class CommandDefinition
    {
        public CommandDefinition(string name, string help, IEnumerable<Argument> arguments,
                                 IEnumerable<CommandExample> examples)
        {
            Name = name;
            Help = help;
            Examples = examples;
            Arguments = new ArgumentCollection(arguments);
        }

        public CommandDefinition(string name, string help, IEnumerable<Argument> arguments,
                                 IEnumerable<CommandExample> examples, CommandDefinition baseDefinition) 
            : this(name, help, arguments, examples)
        {
            BaseDefinition = baseDefinition;
        }

        public string Name { get; }
        public string Help { get; }
        public IEnumerable<CommandExample> Examples { get; }
        public ArgumentCollection Arguments { get; }
        public CommandDefinition BaseDefinition { get; }

        public T Argument<T>(string name) where T : Argument
        {
            return Arguments.OfType<T>().FirstOrDefault(a => a.Name == name);
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Help)}: {Help}, {nameof(Arguments)}: {Arguments}, {nameof(BaseDefinition)}: {BaseDefinition}";
        }
    }
}
