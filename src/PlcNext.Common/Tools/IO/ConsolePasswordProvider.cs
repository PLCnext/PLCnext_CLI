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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcNext.Common.Tools.IO
{
    internal class ConsolePasswordProvider : IPasswordProvider
    {
        public string ProvidePassword()
        {
            string result = string.Empty;
            ConsoleKeyInfo keyInfo;
            Console.WriteLine("Enter password to access keyfile:");
            do
            {
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (result.Length > 0)
                    {
                        Console.Write("\b \b");
                        result = result.Substring(0, result.Length - 1);
                    }
                }
                else
                {

                    if (keyInfo.Key != ConsoleKey.Enter
                        && !char.IsControl(keyInfo.KeyChar))
                    {
                        Console.Write("*");
                        result += keyInfo.KeyChar;
                    }
                }

            } while (keyInfo.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return result;
        }
    }
}
