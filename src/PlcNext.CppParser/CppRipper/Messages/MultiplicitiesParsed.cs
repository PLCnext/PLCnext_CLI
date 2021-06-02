#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;
using Agents.Net;

namespace PlcNext.CppParser.CppRipper.Messages
{
    internal class MultiplicitiesParsed : Message
    {
        public MultiplicitiesParsed(Message predecessorMessage, int[][] multiplicities): base(predecessorMessage)
        {
            Multiplicities = multiplicities;
        }

        public MultiplicitiesParsed(IEnumerable<Message> predecessorMessages, int[][] multiplicities): base(predecessorMessages)
        {
            Multiplicities = multiplicities;
        }
        
        public int[][] Multiplicities { get; }

        protected override string DataToString()
        {
            return
                $"{nameof(Multiplicities)}: {string.Join(", ", Multiplicities.Select(m => $"({string.Join(", ", m)})"))}";
        }
    }
}
