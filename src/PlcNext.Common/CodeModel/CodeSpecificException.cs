#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Globalization;
using System.IO;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.CodeModel
{
    public class CodeSpecificException : FormattableException
    {
        public string CodeFile { get; set; }
        public VirtualDirectory ProjectRoot { get; set; }

        private readonly int line;
        private readonly int? column;
        private readonly object[] additionalInformation;
        private readonly string errorCode;

        public CodeSpecificException(string codeFile, string errorCode, int line,
                                     int? column = null, params object[] additionalInformation)
        {
            CodeFile = codeFile;
            this.line = line;
            this.column = column;
            this.additionalInformation = additionalInformation;
            this.errorCode = errorCode;
        }

        public CodeSpecificException(string errorCode, int line, int? column = null, params object[] additionalInformation)
        {
            this.line = line;
            this.column = column;
            this.errorCode = errorCode;
            this.additionalInformation = additionalInformation;
        }

        public override string Message => column != null
                                              ? $"{GetRelativePath()}({line},{column}): error {errorCode}: {GetMessage()}"
                                              : $"{GetRelativePath()}({line}): error {errorCode}: {GetMessage()}";

        private string GetRelativePath()
        {
            string projectRoot =
                ProjectRoot.FullName.EndsWith($"{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                    ? ProjectRoot.FullName
                    : $"{ProjectRoot.FullName}{Path.DirectorySeparatorChar}";
            Uri baseUri = new Uri(projectRoot, UriKind.Absolute);
            Uri specificUri = new Uri(CodeFile, UriKind.Absolute);
            return baseUri.MakeRelativeUri(specificUri).ToString();
        }

        private string GetMessage()
        {
            string message = CodeErrors.ResourceManager.GetString(errorCode);
            if (additionalInformation != null)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, additionalInformation);
            }

            return message;
        }
    }
}
