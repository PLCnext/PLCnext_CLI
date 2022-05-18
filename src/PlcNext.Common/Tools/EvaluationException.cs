#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Globalization;

namespace PlcNext.Common.Tools
{
    public class EvaluationException : FormattableException
    {
        public EvaluationException(string expression) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.EvaluationFailure, expression))
        {

        }
    }
}
