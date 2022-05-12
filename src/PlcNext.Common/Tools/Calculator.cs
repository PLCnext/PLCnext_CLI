#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using NCalc;

namespace PlcNext.Common.Tools
{
    public static class Calculator
    {
        public static object Evaluate(string expression)
        {
            return new Expression(expression).Evaluate();
        }
    }
}
