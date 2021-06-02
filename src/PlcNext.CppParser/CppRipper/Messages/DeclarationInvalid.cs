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

namespace PlcNext.CppParser.CppRipper.Messages
{
    internal class DeclarationInvalid : MessageDecorator
    {
        private DeclarationInvalid(Message decoratedMessage, ParserMessage errorMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
            ErrorMessage = errorMessage;
        }

        public static DeclarationInvalid Create(Message predecessor, ParserMessage errorMessage = null)
        {
            return new DeclarationInvalid(FieldSetConstructed.Empty(predecessor), errorMessage, new[] {predecessor});
        }
        
        public ParserMessage ErrorMessage { get; }

        protected override string DataToString()
        {
            return $"{nameof(ErrorMessage)}: {ErrorMessage}";
        }
    }
}