#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Iec61131Lib;
using System.Runtime.InteropServices;
using Eclr;
using Iec61131.Engineering.Prototypes.Types;
using Iec61131.Engineering.Prototypes.Variables;
using Iec61131.Engineering.Prototypes.Methods;
using Iec61131.Engineering.Prototypes.Common;
using Iec61131.Engineering.Prototypes.Pragmas;

namespace SharedNative
{
    [Native]
    [FunctionBlock]
    public class NativeFunctionBlock
    {
        [Input]
        public short IN1;
        [Input]
        public short IN2;
        [Output, DataType("WORD")]
        public ushort OUT;

        [Initialization]
        public void __Init()
        {
            //  No implementation in C# part; implement in native method
        }

        [Execution]
        public void __Process()
        {
            // No implementation in C# part; implement in native method
        }
    }
}
