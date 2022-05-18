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

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    internal class SymbolResolveException : FormattableException
    {
        public SymbolResolveException(string symbolName, string parentName) 
            : base($"The symbol {symbolName} in {parentName} could not be resolved. Only elements from the same enum are allowed.")
        {

        }
    }
}
