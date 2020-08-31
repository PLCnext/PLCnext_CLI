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
using System.Text;

namespace PlcNext.Common.Templates
{
    internal class DataChunk
    {
        public DataChunk(string start, string end)
        {
            Start = start;
            End = end;
        }

        public string Start { get; }
        public string End { get; }
    }
}
