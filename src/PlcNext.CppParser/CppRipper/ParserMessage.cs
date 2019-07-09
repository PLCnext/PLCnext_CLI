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
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.CppParser.CppRipper
{
    internal class ParserMessage
    {
        private readonly string errorCode;
        private readonly int line;
        private readonly int? column;
        private readonly string additionalInformation;

        public ParserMessage(string errorCode, int line, int? column = null, string additionalInformation = null)
        {
            this.errorCode = errorCode;
            this.line = line;
            this.column = column;
            this.additionalInformation = additionalInformation;
        }

        public CodeSpecificException ToException(VirtualFile file)
        {
            return new CodeSpecificException(file.FullName, errorCode, line, column, additionalInformation);
        }
    }
}
