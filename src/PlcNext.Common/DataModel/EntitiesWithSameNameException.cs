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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcNext.Common.DataModel
{
    internal class EntitiesWithSameNameException : FormattableException
    {
        public EntitiesWithSameNameException(string name) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.EntitiesWithSameName, name))
        {
            
        }
    }
}
