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
    public class BoolArgument : Argument
    {
        public BoolArgument(string name, char shortName, bool mandatory, string help, string setName) :
            base(name, shortName, mandatory, null, help, setName)
        {
        }

        public bool Value { get; set; }

        public override bool IsDefined { get; protected set; }

        public override void SetValue(object value)
        {
            if (value is bool boolValue)
            {
                IsDefined = true;
                Value = boolValue;
            }
            else if(value != null)
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
