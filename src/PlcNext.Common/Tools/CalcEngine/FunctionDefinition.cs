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

namespace CalcEngine
{
    /// <summary>
    /// Function definition class (keeps function name, parameter counts, and delegate).
    /// </summary>
    internal class FunctionDefinition
    {
        // ** fields
        public int ParmMin, ParmMax;
        public CalcEngineFunction Function;

        // ** ctor
        public FunctionDefinition(int parmMin, int parmMax, CalcEngineFunction function)
        {
            ParmMin = parmMin;
            ParmMax = parmMax;
            Function = function;
        }
    }
}
