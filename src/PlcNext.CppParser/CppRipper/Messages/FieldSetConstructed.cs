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
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;

namespace PlcNext.CppParser.CppRipper.Messages
{
    internal class FieldSetConstructed : Message
    {
        public FieldSetConstructed(Message predecessorMessage, IReadOnlyCollection<CppField> fieldSet, int fileIndex): base(predecessorMessage)
        {
            FieldSet = fieldSet;
            FileIndex = fileIndex;
        }

        public FieldSetConstructed(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<CppField> fieldSet, int fileIndex): base(predecessorMessages)
        {
            FieldSet = fieldSet;
            FileIndex = fileIndex;
        }

        public static FieldSetConstructed Empty(Message predecessorMessage) =>
            new FieldSetConstructed(predecessorMessage, Array.Empty<CppField>(), -1);

        public IReadOnlyCollection<CppField> FieldSet { get; }
        
        public int FileIndex { get; }
        
        protected override string DataToString()
        {
            return $"{nameof(FieldSet)}: {string.Join(", ", FieldSet)}; {nameof(FileIndex)}: {FileIndex}";
        }
    }
}
