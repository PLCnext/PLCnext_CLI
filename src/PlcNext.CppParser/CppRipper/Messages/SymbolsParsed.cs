#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;

namespace PlcNext.CppParser.CppRipper.Messages
{
    internal class SymbolsParsed : Message
    {
        public SymbolsParsed(Message predecessorMessage, IReadOnlyCollection<CppSymbol> symbols): base(predecessorMessage)
        {
            Symbols = symbols;
        }

        public SymbolsParsed(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<CppSymbol> symbols): base(predecessorMessages)
        {
            Symbols = symbols;
        }
        
        public IReadOnlyCollection<CppSymbol> Symbols { get; }

        protected override string DataToString()
        {
            return $"{nameof(Symbols)}: {Symbols}";
        }
    }
}
