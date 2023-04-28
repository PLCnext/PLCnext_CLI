#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.Common.Tools
{
    public static class Calculator
    {
        public static object Evaluate(string expression)
        {
            try
            {
                return new NCalc.Expression(expression).Evaluate();
            }
            catch (Exception)
            {
                throw new EvaluationException(expression);
            }
        }
    }
}
