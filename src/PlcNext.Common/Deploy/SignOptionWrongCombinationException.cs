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

namespace PlcNext.Common.Deploy
{
    internal class SignOptionWrongCombinationException : FormattableException
    {
        public SignOptionWrongCombinationException(string option1, string option2) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.SignOptionWrongCombination, option1, option2))
        {
            
        }
    }
}
