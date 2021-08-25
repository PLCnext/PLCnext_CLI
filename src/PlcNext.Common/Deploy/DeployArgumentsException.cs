#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;

namespace PlcNext.Common.Deploy
{
    internal class DeployArgumentsException : FormattableException
    {
        public DeployArgumentsException() : base(ExceptionTexts.DeployArgumentsWrongCombined)
        {

        }
    }
}
