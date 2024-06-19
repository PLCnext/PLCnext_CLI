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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcNext.Common.Deploy
{
    internal class SignOptionMissingTimestampDecisionException : FormattableException
    {
        public SignOptionMissingTimestampDecisionException() : base(ExceptionTexts.SignOptionMissingTimestampDecision)
        {
            
        }
    }
}
