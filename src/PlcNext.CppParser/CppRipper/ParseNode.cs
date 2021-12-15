
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlcNext.CppParser.CppRipper
{
    /// <summary>
    /// A ParseNode, is a node in the abstract syntax tree. It represents a substring 
    /// of the parsed text, and an associated rule. 
    /// It contains  a pointer into the input text, and an a pair of integers that 
    /// represents the begin index and end index into the text managed by this node.
    /// This is much more efficient than holding copies of the sub-strings, which would consume
    /// much more memory (Log(k,n) * n) instead of n.
    /// ParseNodes are "k-ary" trees. A ParseNode implements "IEnumerable" so that its children
    /// can be enumerated directly. 
    /// </summary>
    public class ParseNode : IEnumerable<ParseNode>
    {
        #region fields 
        /// <summary>
        /// A list of child nodes. This is populated when a node calls "CompleteNode" where 
        /// a node calls "parent.AddChild(this)" .
        /// </summary>
        List<ParseNode> children = new List<ParseNode>();        
        /// <summary>
        /// A parent node, is equal to null if this is the root node in the tree. 
        /// </summary>
        ParseNode parent;
        /// <summary>
        /// The text being parsed 
        /// </summary>
        string text;
        /// <summary>
        /// The beginning index of the text represented by this node 
        /// </summary>
        int begin;
        /// <summary>
        /// The end index of the text represented by this node 
        /// </summary>
        int end;
        /// <summary>
        /// The associated rule. Could be null in the special case of the root node of a parse tree.
        /// </summary>
        Rule rule;
        #endregion 

        /// <summary>
        /// This is only the first step in constructing a parse node.
        /// To complete the construction, "Complete" must be called.
        /// </summary>
        /// <param name="rule">The rule associated with this node</param>
        /// <param name="parent">The parent node, or null if this is the root node</param>
        /// <param name="text">The text being parsed</param>
        /// <param name="begin">The index into the text, which is associated with the parse node</param>
        public ParseNode(Rule rule, ParseNode parent, string text, int begin)
        {
            this.rule = rule;
            this.parent = parent;
            this.text = text;
            this.begin = begin;
            end = begin;
        }

        /// <summary>
        /// Returns the type of the associated rule, or "" if no rule is available
        /// (which is the case for root nodes).
        /// </summary>
        public string RuleType
        {
            get
            {
                if (rule == null) return "_root_";
                return rule.RuleType;
            }
        }

        /// <summary>
        /// Returns the type of the associated rule, or "" if no rule is available
        /// (which is the case for root nodes).
        /// </summary>
        public string RuleName
        {
            get
            {
                if (rule == null) return "_root_";
                return rule.RuleName;
            }
        }

        /// <summary>
        /// Called once the associated parsing rule has successfully completed 
        /// its matching process. This sets the end-index of the rule and adds
        /// the node to its parents node list.
        /// </summary>
        /// <param name="end">The index into the text that represents the extent of text represented by this node</param>
        public void Complete(int end)
        {
            // Make sure that "end" isn't before "begin".
            Trace.Assert(end >= begin);

            // Set the end point 
            this.end = end;

            // If there is a valid parent, we add ourselves to the parent.
            // We also cannot be a skip rule
            if (parent != null)
                parent.AddChild(this);

            // If we are a leaf-rule, we should have no children.
            //if (rule is LeafRule)
            //    Trace.Assert(children.Count == 0);
        }

        public (int line, int column) Position
        {
            get
            {
                int line = 1;
                int column = 1;
                for (int i = 0; i < begin; ++i)
                {
                    if (text[i] == '\n')
                    {
                        column = 1;
                        ++line;
                    }
                    else if (text[i] == '\t')
                    {
                        column += 4;
                    }
                    else
                    {
                        ++column;
                    }
                }

                return (line, column);
            }
        }

        /// <summary>
        /// Returns a substring of the parsed text, which is represented by this node.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (end > begin)
                return text.Substring(begin, end - begin);
            return "";
        }

        /// <summary>
        /// Adds a child node to the parse tree.
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(ParseNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Trace.Assert(node != null);
            Trace.Assert(!(node.rule is SkipRule));
            
            // Add the child of a "NoFailRule", rather than a NoFailRule itself.
            // also if the child rule is an unnamed "ChoiceRule", add its child instead. 
            while (node.rule is NoFailRule || (node.rule.IsUnnamed() && node.rule is ChoiceRule))
            {
                if (node.Count == 0)
                    return;
                if (node.Count > 1)
                    throw new Exception("Internal error: unexpected number of child nodes");
                node = node[0];
            }

            children.Add(node);
        }

        /// <summary>
        /// Returns the number of child nodes
        /// </summary>
        public int Count
        {
            get { return children.Count; }
        }

        /// <summary>
        /// Provides access to child nodes indexed by integer
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public ParseNode this[int n]
        {
            get { return children[n]; }
        }

        /// <summary>
        /// Provides access to children indexed by the rule name
        /// </summary>
        /// <param name="s">The rule name</param>
        /// <returns></returns>
        public ParseNode this[string s] 
        {
            get 
            {
                foreach (ParseNode pn in children)
                    if (pn.RuleName == s)
                        return pn;
                return null;
            }
        }

        /// <summary>
        /// Returns true if the underlying rule is unnamed
        /// </summary>
        /// <returns></returns>
        public bool IsUnnamed()
        {
            if (rule != null)
                return rule.IsUnnamed();
            return true;
        }

        #region IEnumerable<ParseNode> Members

        /// <summary>
        /// Exposes an enumerator over the child nodes 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ParseNode> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Exposes an enumerator over the child nodes 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return children.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Returns the parent if it is a named Rule, otherwise returns the 
        /// first ancestor which has a name
        /// </summary>
        /// <returns></returns>
        public ParseNode GetNamedParent()
        {
            if (parent == null)
                return null;
            if (!parent.IsUnnamed())
                return parent;
            return parent.GetNamedParent();
        }

        /// <summary>
        /// Returns a pointer to the parent node.
        /// </summary>
        /// <returns></returns>
        public ParseNode GetParent()
        {
            return parent;
        }

        /// <summary>
        /// Returns the index of this node in the parent collection.
        /// </summary>
        /// <returns></returns>
        public int GetParentIndex()
        {
            return parent.children.IndexOf(this);
        }

        /// <summary>
        /// Returns the rule associated with this node
        /// </summary>
        /// <returns></returns>
        public Rule GetRule()
        {
            return rule;
        }

        /// <summary>
        /// Returns true if the parse rule corresponds to no actual text 
        /// </summary>
        public bool IsZeroWidth
        {
            get { return begin == end; }
        }

        /// <summary>
        /// Creates an iterator over all sub-items and child items.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParseNode> GetHierarchy()
        {
            foreach (ParseNode node in this)
                foreach (ParseNode child in node.GetHierarchy())
                    yield return child;
            yield return this;
        }


    }
}
