#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PlcNext.Common.Commands
{
    public class SetTargetsOptionMissingException : FormattableException
    {
        public SetTargetsOptionMissingException() : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.SetTargetsOptionMissing))
        {

        }
    }
}
