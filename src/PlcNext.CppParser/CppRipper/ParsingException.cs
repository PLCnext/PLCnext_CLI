
using System;

namespace PlcNext.CppParser.CppRipper
{
    public class ParsingException
        : Exception
    {
        private readonly Rule failedRule;
        private readonly Rule parentRule;
        public int Col { get; }
        public int Row { get; }
        private readonly string line;
        private readonly string ptr;

        public ParsingException(ParseNode parent, Rule rule, ParserState ps)
        {
            if (ps == null)
            {
                throw new ArgumentNullException(nameof(ps));
            }

            // Store the failed node, the parent node (which should be named), and the associated rule
            ParseNode parentNode = parent;
            if (parentNode != null)
                parentNode = parentNode.GetNamedParent();
            failedRule = rule;
            if (parentNode != null)
                parentRule = parentNode.GetRule();

            // set the main text variables
            string text = ps.Text;

            // set the index into the text
            int index = ps.Index;
            if (index >= text.Length)
                index = text.Length - 1;

            // initialize a bunch of values 
            int lineStart = 0;
            Col = 0;
            Row = 0;
            int i = 0;

            // Compute the column, row, and lineStart
            for (; i < index; ++i)
            {
                if (text[i] == '\n')
                {
                    lineStart = i + 1;
                    Col = 0;
                    ++Row;
                }
                else
                {
                    ++Col;
                }
            }

            // Compute the line end
            while (i < text.Length)
                if (text[i++] == '\n')
                    break;

            // Compute the line length 
            int lineLength = i - lineStart;

            // Get the line text (don't include the new line)
            line = text.Substring(lineStart, lineLength - 1);
            
            // Assume Tabs of length of four
            string tab = "    ";

            // Compute the pointer (^) line will be
            // based on the fact that we will be replacing tabs 
            // with spaces.
            string tmp = line.Substring(0, Col);
            tmp = tmp.Replace("\t", tab);
            ptr = new String(' ', tmp.Length);
            ptr += "^";

            // Replace tabs with spaces
            line = line.Replace("\t", tab);
        }

        public string Location
        {
            get
            {
                string s = "line number " + Row + ", and character number " + Col + "\n";
                s += line + "\n";
                s += ptr + "\n";
                return s;
            }
        }

        public override string Message
        {
            get
            {
                return ToString();
            }
        }

        public override string ToString()
        {
            string s = "parsing exception occured ";
            if (parentRule != null)
            {
                s += "while parsing '" + parentRule + "' ";
            }
            if (failedRule != null)
                s += "expected '" + failedRule + "' ";
            s += " at \n";
            s += Location;
            return s;
        }
        
    }
}
