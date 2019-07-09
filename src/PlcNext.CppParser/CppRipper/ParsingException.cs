#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.CppParser.CppRipper
{
    public class ParsingException
        : Exception
    {
        public int location;
        public ParseNode parentNode;
        public Rule failedRule;
        public Rule parentRule;
        public int col;
        public int row;
        public int index;
        public int lineStart;
        public int lineEnd;
        public int lineLength;
        public string line;
        public string text;
        public string ptr;

        public ParsingException(ParseNode parent, Rule rule, ParserState ps)
        {
            // Store the failed node, the parent node (which should be named), and the associated rule
            parentNode = parent;
            if (parentNode != null)
                parentNode = parentNode.GetNamedParent();
            failedRule = rule;
            if (parentNode != null)
                parentRule = parentNode.GetRule();

            // set the main text variables
            text = ps.text;

            // set the index into the text
            index = ps.index;
            if (index >= text.Length)
                index = text.Length - 1;

            // initialize a bunch of values 
            lineStart = 0;
            col = 0;
            row = 0;
            int i = 0;

            // Compute the column, row, and lineStart
            for (; i < index; ++i)
            {
                if (text[i] == '\n')
                {
                    lineStart = i + 1;
                    col = 0;
                    ++row;
                }
                else
                {
                    ++col;
                }
            }

            // Compute the line end
            while (i < text.Length)
                if (text[i++] == '\n')
                    break;
            lineEnd = i;

            // Compute the line length 
            lineLength = lineEnd - lineStart;

            // Get the line text (don't include the new line)
            line = text.Substring(lineStart, lineLength - 1);
            
            // Assume Tabs of length of four
            string tab = "    ";

            // Compute the pointer (^) line will be
            // based on the fact that we will be replacing tabs 
            // with spaces.
            string tmp = line.Substring(0, col);
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
                string s = "line number " + row.ToString() + ", and character number " + col.ToString() + "\n";
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
                s += "while parsing '" + parentRule.ToString() + "' ";
            }
            if (failedRule != null)
                s += "expected '" + failedRule.ToString() + "' ";
            s += " at \n";
            s += Location;
            return s;
        }
        
    }
}
