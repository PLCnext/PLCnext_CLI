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
}

