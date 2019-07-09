#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;

namespace PlcNext.CppParser.CppRipper
{
    public class CppStreamParser
    {
        private static readonly CppStructuralGrammar Grammar = new CppStructuralGrammar();

        public bool Succeeded { get; private set; }
        public ParsingException Exception { get; private set; }

        public ParseNode Parse(Stream stream)
        {
            Rule parseRule = Grammar.file;
            string text;
            using (TextReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            ParserState state = new ParserState(text);

            try
            {
                Succeeded = parseRule.Match(state) && state.AtEndOfInput();
            }
            catch (ParsingException e)
            {
                state.ForceCompletion();
                Succeeded = false;
                Exception = e;
            }

            return state.GetRoot();
        }
    }

    public class CppFileParser
    {
        static CppStructuralGrammar grammar = new CppStructuralGrammar();

        private string message;

        public string Message { get { return message; } }

        public CppFileParser(IAstPrinter printer, string file)
        {
            Rule parse_rule = grammar.file;

            string text = File.ReadAllText(file);
            printer.Clear();
            ParserState state = new ParserState(text);

            try
            {
                if (!parse_rule.Match(state))
                {
                    message = "Failed to parse file " + file;
                }
                else
                {
                    if (state.AtEndOfInput())
                    {
                        message = "Successfully parsed file";
                    }
                    else
                    {
                        message = "Failed to read end of input";
                    }
                }
                    
            }
            catch (ParsingException e)
            {
                state.ForceCompletion();
                message = e.Message;
            }

            printer.PrintNode(state.GetRoot(), 0);
        }
    }

    public class CppFileSetParser
    {
        public DirectoryInfo di;

        public CppFileSetParser(IAstPrinter printer, string sDir)
        {
            di = new DirectoryInfo(sDir);
            foreach (FileInfo fi in di.GetFiles("*.c;*.cpp;*.h;*.hpp;"))
            {
                CppFileParser fp = new CppFileParser(printer, fi.Name);
            }
        }
    }
}

