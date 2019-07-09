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
using System.Text;

namespace PlcNext.Common.Build
{
    internal class WrongCombinedExternalLibrariesException : FormattableException
    {
        public WrongCombinedExternalLibrariesException() : base(ExceptionTexts.WrongCombinedExternalLibraries)
        {

        }
    }
}
