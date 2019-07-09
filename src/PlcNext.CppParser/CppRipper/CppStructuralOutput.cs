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

namespace PlcNext.CppParser.CppRipper
{
    /// <summary>
    /// A parser will print to any class that implements IAstPrinter.
    /// This is effectively a pretty printer.
    /// </summary>
    public interface IAstPrinter
    {
        void Clear();
        void PrintNode(ParseNode node, int depth);
    }

    /// <summary>
    /// A base class that contains common functionality for printing 
    /// an AST tree to text.
    /// </summary>
    public class AstTextPrinter
        : IAstPrinter
    {
        List<string> strings = new List<string>();
        protected string indent;

        public String[] GetStrings()
        {
            return strings.ToArray();
        }

        public void Write(string s)
        {
            if (strings.Count > 0)
            {
                string tmp = strings[strings.Count - 1];
                strings[strings.Count - 1] = tmp + s;
            }
            else
            {
                strings.Add(indent + s);
            }
        }

        /// <summary>
        /// Puts the string 
        /// </summary>
        /// <param name="s"></param>
        public void WriteLine(string s)
        {
            strings.Add(indent + s);
        }

        /// <summary>
        /// Add a new blank line with indenting.
        /// </summary>
        public void WriteLine()
        {
            WriteLine("");
        }

        /// <summary>
        /// Used to set a single breakpoint for an assertion failure. Facilitates debugging.  
        /// </summary>
        /// <param name="b"></param>
        public void Assert(bool b)
        {
            if (!b)
                throw new Exception("Assertion failed");
        }

        #region IAstPrinter Members
        public void Clear()
        {
            strings.Clear();
        }

        public virtual void PrintNode(ParseNode node, int depth)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// A pretty printer designed for the structural C++ grammar
    /// </summary>
    public class CppStructuralOutput
        : AstTextPrinter
    {
        public void PrintSimpleNode(ParseNode pn)
        {
            string s = "<" + pn.RuleType;
            if (!pn.IsUnnamed())
                s += " name='" + pn.RuleName + "'";
            s += ">";
            s += pn.ToString();
            s += "</" + pn.RuleType + ">";
            WriteLine(s);
        }

        public override void PrintNode(ParseNode pn, int depth)
        {
            indent = new String(' ', depth * 2);
            if (pn.IsUnnamed())
            {
                foreach (ParseNode child in pn)
                    PrintNode(child, depth);
            }
            else if (pn.RuleName == "pp_directive")
            {
                PrintSimpleNode(pn);
            }
            else if (pn.RuleName == "comment_set")
            {
                PrintSimpleNode(pn);
            }
            else
            {
                string s = "<" + pn.RuleType;
                if (!pn.IsUnnamed())
                    s += " name='" + pn.RuleName + "'";
                s += ">";
                WriteLine(s);

                if (pn.Count == 0)
                {
                    WriteLine(pn.ToString());
                }
                else
                {
                    foreach (ParseNode tmp in pn)
                        PrintNode(tmp, depth + 1);
                }
                WriteLine("</" + pn.RuleType + ">");
            }
        }

        public string TypeDeclToString(ParseNode node)
        {
            string r = "";
            Assert(node.RuleName == "type_decl");
            Assert(node.RuleType == "sequence");
            Assert(node.Count == 2);
            Assert(node[1].RuleType == "choice");
            Assert(node[1].Count == 1);
            switch (node[1][0].RuleName)
            {
                case "class_decl":
                case "struct_decl":
                case "union_decl":
                case "enum_decl":
                    r += node[1][0].ToString() + " ";
                    Assert(node[1][0].Count >= 3);
                    Assert(node[1][0][1].RuleType == "opt");
                    ParseNode optIdent = node[1][0][1];
                    if (optIdent.Count > 0)
                    {
                        Assert(optIdent[0].RuleName == "identifier");
                        r += optIdent[0].ToString();
                    }
                    else
                    {
                        r += "_anon_";
                    }
                    break;
                default:
                    throw new Exception("Unrecognized type declaration");
            }
            return r;
        }

        public void OutputDeclarationContent(ParseNode node, string indent)
        {
            Assert(node.RuleName == "declaration_content");
            string s = "content: ";
            foreach (ParseNode child in node)
            {
                Assert(child.Count == 2);
                Assert(child[0].RuleName == "node");
                Assert(child[0].RuleType == "choice");
                Assert(child[0].Count == 1);

                ParseNode tmp = child[0][0];
                switch (tmp.RuleName)
                {
                    case "bracketed_group":
                        s += "[...] ";
                        break;
                    case "paran_group":
                        s += "(...) ";
                        break;
                    case "brace_group":
                        s += "{...} ";
                        break;
                    case "type_decl":
                        s += TypeDeclToString(tmp);
                        break;
                    case "typedef_decl":
                        s += "typdef ";
                        break;
                    case "literal":
                    case "symbol":
                    case "label":
                    case "identifier":
                        s += tmp.ToString() + " ";
                        break;
                }
            }
            WriteLine(s);
        }

        public void OutputPrefixComment(ParseNode node, string indent)
        {
            Assert(node.RuleName == "comment_set");
            string r = "";

            foreach (ParseNode x in node.GetHierarchy())
                if (x.RuleName == "comment")
                    r += x.ToString();

            if (r.Length > 0)
                WriteLine("prefix comment: " + r);
        }

        public void OutputSuffixComment(ParseNode node, string indent)
        {
        }

        public void OutputDeclaration(ParseNode node, string indent)
        {
            if (node.RuleName == "declaration")
            {
                Assert(node.Count == 1);
                node = node[0];
                Assert(node.Count >= 2);
                Assert(node[0].RuleName == "comment_set");
                OutputPrefixComment(node[0], indent);

                switch (node[1].RuleName)
                {
                    case "pp_directive":
                        WriteLine("pp_directive: " + node[1].ToString());
                        break;
                    case "declaration_content":
                        OutputSuffixComment(node[3], indent);
                        OutputDeclarationContent(node[1], indent);
                        break;
                    case "semicolon":
                        WriteLine("empty declaration");
                        break;
                    default:
                        throw new Exception("Unrecognized kind of declaration");
                }
            }
            foreach (ParseNode child in node)
                OutputDeclaration(child, indent + "  ");
        }
    }
}
