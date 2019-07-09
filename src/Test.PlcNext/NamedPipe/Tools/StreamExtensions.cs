#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Test.PlcNext.NamedPipe.Tools
{
    public static class StreamExtensions
    {
        public static Stream ConvertPaths(this Stream input)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return input;
            }

            string content;
            Encoding encoding = Encoding.UTF8;
            using (StreamReader reader = new StreamReader(input, encoding, true, 4098, false))
            {
                content = reader.ReadToEnd();
                encoding = reader.CurrentEncoding;
            }

            return new MemoryStream(encoding.GetBytes(content.Replace("C:\\\\MySdk", "/usr/bin/MySdk")
                                                             .Replace("C:/MySdk", "/usr/bin/MySdk")));
        }
    }
}