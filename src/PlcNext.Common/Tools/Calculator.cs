﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

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
            catch (NCalc.EvaluationException)
            {
                throw new EvaluationException(expression);
            }
        }
    }
}