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
using System.Linq;
using System.Text;

namespace PlcNext.Common.Tools.UI
{
    internal class OutputFormatterPool : IOutputFormatterPool
    {
        private readonly IEnumerable<IOutputFormatter> formatters;

        public OutputFormatterPool(IEnumerable<IOutputFormatter> formatters)
        {
            this.formatters = formatters;
        }

        public IUserInterface GetFormatter(FormatterParameters parameters, IUserInterface wrappedUserInterface)
        {
            IOutputFormatter formatter = formatters.FirstOrDefault(f => f.CanFormat(parameters));
            return formatter?.Format(wrappedUserInterface) ?? wrappedUserInterface;
        }
    }
}
