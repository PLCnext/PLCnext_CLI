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
    public class SingleValueArgument : Argument
    {
        private string value;

        public SingleValueArgument(string name, char shortName, bool mandatory, Func<string, (bool, string, string)> restriction, string help)
            : base(name, shortName, mandatory, restriction, help)
        {
        }

        public string Value
        {
            get => value;
            set
            {
                if (Restriction != null)
                {
                    (bool success, string message, string newValue) = Restriction(value);
                    if (!success)
                    {
                        throw new ArgumentRestrictionException(this, value, message);
                    }

                    this.value = newValue;
                }
                else
                {
                    this.value = value;
                }
            }
        }

        public override bool IsDefined { get; protected set; }

        public override void SetValue(object obj)
        {
            if (obj is string stringValue)
            {
                IsDefined = true;
                Value = stringValue;
            }
            else if (obj != null)
            {
                throw new ArgumentTypeMismatchException();
            }
        }
        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Value)}: {Value}";
        }
    }
}
