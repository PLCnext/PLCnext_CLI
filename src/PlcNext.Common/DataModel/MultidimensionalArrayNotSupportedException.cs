#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;

namespace PlcNext.Common.DataModel
{
    internal class MultidimensionalArrayNotSupportedException : FormattableException
    {
        public MultidimensionalArrayNotSupportedException() : base(ExceptionTexts.MultidimensionalArrayNotSupported)
        {

        }
    }
}
