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
    public class MultipleValueArgument : Argument
    {
        private IEnumerable<string> values;

        public MultipleValueArgument(string name, char shortName, bool mandatory, Func<string, (bool, string, string)> restriction,
                                     string help, string setName, char separator) : base(name, shortName, mandatory, restriction, help, setName)
        {
            Separator = separator;
        }

        public IEnumerable<string> Values
        {
            get => values;
            set
            {
                if (Restriction != null)
                {
                    List<string> result = new List<string>(value);
                    foreach (string v in result.ToArray())
                    {
                        (bool success, string message, string newValue) = Restriction(v);
                        if (!success)
                        {
                            throw new ArgumentRestrictionException(this, v, message);
                        }

                        int index = result.IndexOf(v);
                        result.RemoveAt(index);
                        result.Insert(index, newValue);
                    }
                    values = result;
                }
                else
                {
                    values = value;
                }
            }
        }

        public override bool IsDefined { get; protected set; }
        public char Separator { get; }

        public override void SetValue(object obj)
        {
            if (obj is IEnumerable<string> stringValues)
            {
                if (stringValues.Any())
                {
                    IsDefined = true;
                    Values = stringValues;
                }
            }
            else if(obj != null)
            {
                throw new ArgumentTypeMismatchException();
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Values)}: {string.Join(",", Values ?? new string[0])}";
        }
    }
}
