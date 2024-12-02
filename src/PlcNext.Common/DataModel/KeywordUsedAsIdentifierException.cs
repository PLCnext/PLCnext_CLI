#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;
using System.Globalization;

namespace PlcNext.Common.DataModel
{
    internal class KeywordUsedAsIdentifierException : FormattableException
    {
        public KeywordUsedAsIdentifierException(string keyword) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.KeywordUsedAsIdentifier, keyword))
        {
            
        }
    }
}
