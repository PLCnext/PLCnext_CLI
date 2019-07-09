#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Linq;

namespace PlcNext.NamedPipeServer.Communication
{
    public static class EndiannessTools
    {
        public static byte[] BigEndian(this byte[] data)
        {
            return BitConverter.IsLittleEndian != false ? data.Reverse().ToArray() : data;
        }
    }
}
