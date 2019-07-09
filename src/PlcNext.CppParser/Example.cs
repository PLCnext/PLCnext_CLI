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
using System.IO;
using System.Linq;
using System.Text;
using PlcNext.CppParser.CppRipper;

namespace PlcNext.CppParser
{
    public class Example
    {
        public void Main()
        {
            CppStructuralOutput output = new CppStructuralOutput();
            CppFileParser parser = new CppFileParser(output, @"C:\keep\Demo+ws\workspace\CppTestProject\src\Programs\CppTestProgram.hpp");
            foreach (string line in output.GetStrings())
            {
                Console.WriteLine(line);
            }
            Console.WriteLine();
            Console.WriteLine();
            //Error because fo wild comment after class close; should be recognized as same line comment
            Console.WriteLine(parser.Message);
            File.WriteAllLines(@"C:\temp\structure.xml",output.GetStrings());
        }

        private class AstPrinter : IAstPrinter
        {
            public void Clear()
            {
                throw new NotImplementedException();
            }

            public void PrintNode(ParseNode node, int depth)
            {
                throw new NotImplementedException();
            }
        }
    }
}
