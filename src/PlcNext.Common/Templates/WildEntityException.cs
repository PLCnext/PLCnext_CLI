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
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Templates
{
    internal class WildEntityException : FormattableException
    {
        public WildEntityException(IType type, string template, string[] relationships) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.WildEntity, type.FullName,template, string.Join(", ",relationships)))
        {
            
        }
    }
}
