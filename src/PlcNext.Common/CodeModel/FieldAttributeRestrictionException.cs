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
using System.Globalization;
using System.Text;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.CodeModel
{
    internal class FieldAttributeRestrictionException : CodeSpecificException
    {
        public FieldAttributeRestrictionException(string attributeName, string attributeValue, string message, CodePosition position, VirtualFile codeFile) : base(codeFile.FullName, "ARP0001", position.Line, position.Column, attributeName, attributeValue, message)
        {
            
        }
    }
}
