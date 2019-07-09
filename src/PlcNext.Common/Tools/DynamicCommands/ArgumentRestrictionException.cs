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
using System.Globalization;
using System.Text;

namespace PlcNext.Common.Tools.DynamicCommands
{
    internal class ArgumentRestrictionException : FormattableException
    {
        public ArgumentRestrictionException(Argument argument, string value, string message) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.ArgumentRestriction, argument.Name, value, message))
        {
            
        }
    }
}
