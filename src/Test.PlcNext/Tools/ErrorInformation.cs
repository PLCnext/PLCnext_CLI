#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Test.PlcNext.Tools
{
    public class ErrorInformation
    {
        public string Filename { get; }
        public int Line { get; }
        public int Column { get; }
        public string ErrorCode { get; }

        public ErrorInformation(string filename, int line, int column, string errorCode)
        {
            Filename = filename;
            Line = line;
            Column = column;
            ErrorCode = errorCode;
        }

        public override string ToString()
        {
            return $"{ErrorCode}";
        }

        public string ToFullString()
        {
            return $"{Filename} ({Line},{Column}): {ErrorCode}";
        }
    }
}
