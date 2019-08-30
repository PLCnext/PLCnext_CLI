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
    public abstract class Argument
    {
        protected Argument(string name, char shortName, bool mandatory, Func<string, (bool, string, string)> restriction, string help,
                           string setName)
        {
            Name = name;
            ShortName = shortName;
            Restriction = restriction;
            Help = help;
            SetName = setName;
            Mandatory = mandatory;
        }

        public bool Mandatory { get; }
        public string Name { get; }
        public char ShortName { get; }
        public Func<string,(bool success, string message, string newValue)> Restriction { get; }
        public string Help { get; }
        public string SetName { get; }
        public abstract bool IsDefined { get; protected set; }

        public abstract void SetValue(object value);

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(ShortName)}: {ShortName}, {nameof(Help)}: {Help}";
        }
    }
}
