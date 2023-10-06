
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlcNext.CppParser.CppRipper
{
    public class ParserState
    {
        #region private fields
        Stack<ParseNode> nodes = new Stack<ParseNode>();
        #endregion 

        #region public fields

        public string Text { get; }
        public int Index { get; set; }

        #endregion

        #region public propterties
        /// <summary>
        /// Indicates whether nodes should be created. If set, 
        /// it will be applied recursively to all child nodes.
        /// </summary>
        public bool CreateNodes { get; set; }

        /// <summary>
        /// Outputs a number of characters before the current parser position.
        /// When debugging, this property helps us see where we are in the input stream.
        /// </summary>
        internal string DebugPrefixContext
        {
            get
            {
                int cnt = 25;
                int begin = Index - cnt;
                if (begin < 0)
                {
                    begin = 0;
                    cnt = Index - begin;
                }
                return String.Concat("... ", Text.AsSpan(begin, cnt));
            }
        }

        /// <summary>
        /// Outputs a number of characters after the current parser position.
        /// When debugging, this property helps us see where we are in the input stream
        /// </summary>
        internal string DebugSuffixContext
        {
            get
            {
                int cnt = 25;
                if (Index + cnt > Text.Length)
                    cnt = Text.Length - Index;
                return String.Concat(Text.AsSpan(Index, cnt), " ... ");
            }
        }
        #endregion

        /// <summary>
        /// Constructs a parser state, that manages a pointer to the text.
        /// </summary>
        /// <param name="text"></param>
        public ParserState(string text)
        {
            CreateNodes = true;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            ParseNode root = new ParseNode(null, null, text, 0);
            root.Complete(text.Length);
            nodes.Push(root);
        }

        /// <summary>
        /// Returns true if the index is at the end of the input string
        /// </summary>
        /// <returns></returns>
        public bool AtEndOfInput()
        {
            return Index == Text.Length;
        }

        /// <summary>
        /// Returns the root node in the abstract syntax tree
        /// </summary>
        /// <returns></returns>
        public ParseNode GetRoot()
        {
            Trace.Assert(nodes.Count == 1);
            return nodes.Peek();
        }

        /// <summary>
        /// Pushes a node onto the AST stack, which is used for constructing the tree
        /// </summary>
        /// <param name="x"></param>
        public void Push(ParseNode x)
        {
            Trace.Assert(x != null);
            Trace.Assert(x.GetParent() == Peek());
            nodes.Push(x);
        }

        /// <summary>
        /// Returns the top node on the AST stack.
        /// </summary>
        /// <returns></returns>
        public ParseNode Peek()
        {
            return nodes.Peek();
        }

        /// <summary>
        /// Removes the top node from the AST stack
        /// </summary>
        public void Pop()
        {
            nodes.Pop();
        }

        /// <summary>
        /// Will force completion of the parse tree, by completing all nodes on the stack immediately .
        /// Used in the case of an exception to produces a partially complete  tree.
        /// </summary>
        public void ForceCompletion()
        {
            while (nodes.Count > 1)
            {
                Peek().Complete(Index);
                Pop();
            }
        }
    }
}