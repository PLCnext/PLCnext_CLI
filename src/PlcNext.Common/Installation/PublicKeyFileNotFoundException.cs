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
using PlcNext.Common.Tools;

namespace PlcNext.Common.Installation
{
    internal class PublicKeyFileNotFoundException : FormattableException
    {
        public PublicKeyFileNotFoundException() : base(ExceptionTexts.PublicKeyFileMissing)
        {
            
        }
    }
}
