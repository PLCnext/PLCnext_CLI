
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PlcNext.CppParser.CppRipper
{
    /// <summary>
    /// Describes a parsing rule, which maps roughly to a PEG grammar rule. 
    /// Unlike typical CFG forms (like BNF, or extended BNF) a PEG grammar
    /// rule is prioritized, unambiguous, and may have zero length 
    /// The function which a user calls is "Match" which performs the 
    /// actual matching and advancing of the parser index if successful.
    /// The different kinds of rules, provide overrides of the abstract 
    /// function InternalMatch to customize their own behavior.
    /// </summary>
    public abstract class Rule
    {
        #region private fields
        /// <summary>
        /// A Rule can be named or unnamed. An unnamed Rule (IsUnnamed() == true)
        /// mean that the name field is null. The name is set by a call to static function
        /// </summary>
        string Name { get; set; }
        #endregion

        #region protected fields

        #endregion

        #region public methods
        /// <summary>
        /// Used to combine unnamed sequence rules and unamed choice rules, into one single list.
        /// This is because long expressions like "a + b + c" would create a right-heavy binary tree 
        /// (e.g. (a + b) + c) where we really want a single list. 
        /// </summary>
        public virtual void FlattenRules()
        {
            foreach (Rule r in Rules)
                r.FlattenRules();
        }
        /// <summary>
        /// Used to add new sub-rules to SeqRule or ChildRule rules. 
        /// </summary>
        /// <param name="r"></param>
        public void AddRule(Rule r)
        {
            // If this assertion fails, there is a good chance that it is because
            // you are referring to a rule that hasn't been initialized yet. 
            Trace.Assert(r != null);
            Rules.Add(r);
        }
        /// <summary>
        /// Returns a collection of all child rules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Rule> GetRules()
        {
            return Rules;
        }
        #endregion

        #region overrides
        /// <summary>
        /// Returns a string representation of a parsing rule.
        /// myrule ::== (some_rule | some_other_rule)* + terminator;
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsUnnamed())
                return RuleDefinition;
            return Name;
        }
        #endregion

        #region properties
        /// <summary>
        /// Returns the name of the rule, or _unnamed_ if it is an unnamed rule.
        /// </summary>
        public string RuleName
        {
            get
            {
                if (IsUnnamed())
                    return "_unnamed_";
                return Name;
            }
        }

        /// <summary>
        /// Outputs the rule in the form "MyRuleName ::== SomeRule + (OneRule | AnotherRule)"
        /// </summary>
        public string RuleNameAndDefinition
        {
            get
            {
                return RuleName + " ::== " + RuleDefinition;
            }
        }


        /// <summary>
        /// Outputs the rule name, or the definition if unnamed.
        /// </summary>
        public string RuleNameOrDefinition
        {
            get
            {
                if (IsUnnamed())
                    return RuleDefinition;
                return 
                    RuleName;
            }
        }

        /// <summary>
        /// Returns a string that represents the defintion of parsing rule
        /// (some_rule | some_other_rule)* + terminator
        /// </summary>
        public abstract string RuleDefinition { get; }

        /// <summary>
        /// Returns a string representing the kind of rule (Star, Choice, etc.)
        /// </summary>
        public abstract string RuleType { get; }

        /// <summary>
        /// Used to manage child rules. In many cases this is a list of one-element (e.g. StarRule, PlusRule).
        /// In some cases it has no-elements (e.g. NothingRule, AnythingRule). And in the cases of SeqRule, or ChoiceRule
        /// it will contain two or more rules.
        /// </summary>
        protected List<Rule> Rules { get; } = new List<Rule>();

        #endregion

        #region operator overloads
        /// <summary>
        /// Represent the choice operator. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Rule operator |(Rule a, Rule b)
        {
            return new ChoiceRule(a, b);
        }

        /// <summary>
        /// Represents the sequence operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Rule operator +(Rule a, Rule b)
        {
            return new SeqRule(a, b);
        }

        // Note: string a + string b is already to return a concatenated string, 
        // so it is not included here.
        #endregion

        /// <summary>
        /// This is the work-horse function. It attempts to match a rule
        /// by calling the rule's specialized "InternalMatch()" function. 
        /// If the match is successful, a node is created and added to the 
        /// the parser tree managed by ParserState. 
        /// If the match fails, then no node is created (technically 
        /// the node was already created, and just gets deleted) and the 
        /// ParserState index is returned to its original location.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual bool Match(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            int old_index = p.Index;

            if (p.CreateNodes && !(this is SkipRule))
            {
                ParseNode node = new ParseNode(this, p.Peek(), p.Text, old_index);
                p.Push(node);
                if (InternalMatch(p))
                {
                    node.Complete(p.Index);
                    p.Pop();
                    return true;
                }

                p.Index = old_index;
                node = null;
                p.Pop();
                return false;
            }

            if (InternalMatch(p))
            {
                return true;
            }

            p.Index = old_index;
            return false;
        }

        /// <summary>
        /// Each specialized rule type, provides its own definition of InternalMatch 
        /// to do what is expected. An override of this function should not take 
        /// care of creating parse tree nodes, or restoring the text pointer, because
        /// that is done automatically by the Match function.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected abstract bool InternalMatch(ParserState p);

        /// <summary>
        /// Used to assign names. Usually just called by AssignRuleNames,
        /// which reflects over an object's field names to set the rule names.        
        /// </summary>
        /// <param name="s"></param>
        public void SetName(string s)
        {
            Name = s;
        }

        /// <summary>
        /// Returns true if the name field is null.
        /// </summary>
        /// <returns></returns>
        public bool IsUnnamed()
        {
            return Name == null;
        }

        /// <summary>
        /// Returns all child rules.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Rule> GetChildren()
        {
            return Rules;
        }
    }

    /// <summary>
    /// Matches two or more sub-rules, returning true if any are successful.    
    /// </summary>
    public class ChoiceRule : Rule
    {
        public ChoiceRule(Rule a, Rule b)
        {
            AddRule(a);
            AddRule(b);
        }

        public override string RuleDefinition
        {
            get
            {
                string r = "(";
                int i = 0;
                foreach (Rule rule in GetRules())
                {
                    if (i++ > 0) r += " | ";
                    r += rule.ToString();
                }
                return r + ")";
            }
        }

        public override string RuleType
        {
            get { return "choice"; }
        }

        public override void FlattenRules()
        {
            base.FlattenRules();

            while (true)
            {
                List<Rule> tmp = new List<Rule>();
                foreach (Rule r in GetRules())
                    if (r is ChoiceRule && r.IsUnnamed())
                        tmp.AddRange(r.GetRules());
                    else
                        tmp.Add(r);
                if (Rules.Count == tmp.Count)
                    return;
                Rules.Clear();
                Rules.AddRange(tmp);
            }
        }

        protected override bool InternalMatch(ParserState p)
        {
            foreach (Rule r in Rules)
                if (r.Match(p))
                    return true;
            return false;
        }
    }

    /// <summary>
    /// Matches a sequence of rules, one by one.
    /// </summary>
    public class SeqRule : Rule
    {
        public SeqRule(Rule a, Rule b)
        {
            AddRule(a);
            AddRule(b);
        }

        public SeqRule(IEnumerable<Rule> xs)
        {
            Rules.AddRange(xs);

            // We should at least have two sub-rules 
            Trace.Assert(Rules.Count >= 2);
        }

        public override string RuleType
        {
            get { return "sequence"; }
        }

        public override string RuleDefinition
        {
            get
            {
                string r = "(";
                for (int i = 0; i < Rules.Count; ++i)
                {
                    if (i > 0) r += " + ";
                    r += Rules[i].ToString();
                }
                return r + ")";
            }
        }

        public override void FlattenRules()
        {
            base.FlattenRules();

            while (true)
            {
                List<Rule> tmp = new List<Rule>();
                foreach (Rule r in Rules)
                    if (r is SeqRule && r.IsUnnamed())
                        tmp.AddRange(r.GetRules());
                    else
                        tmp.Add(r);
                if (Rules.Count == tmp.Count)
                    return;
                Rules.Clear();
                Rules.AddRange(tmp);
            }
        }

        protected override bool InternalMatch(ParserState p)
        {
            foreach (Rule r in Rules)
                if (!r.Match(p))
                    return false;
            return true;
        }
    }

    /// <summary>
    /// Matches a single character and advances the index by one
    /// </summary>
    public class AnythingRule : Rule
    {
        public static AnythingRule Instance { get; } = new AnythingRule();

        public override string RuleType
        {
            get { return "anything"; }
        }

        public override string RuleDefinition
        {
            get { return "."; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            Trace.Assert(!p.AtEndOfInput());
            p.Index++;
            return true;
        }
    }

    /// <summary>
    /// Always returns true, but unlike AnythingRule does not advance 
    /// the index by one
    /// </summary>
    public class NothingRule : Rule
    {
        public static NothingRule Instance { get; } = new NothingRule();

        public override string RuleType
        {
            get { return "nothing"; }
        }

        public override string RuleDefinition
        {
            get { return "#"; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            return true;
        }
    }

    /// <summary>
    /// Matches a sub-rule 0 or more times.
    /// </summary>
    public class StarRule : Rule
    {
        public override string RuleType
        {
            get { return "star"; }
        }

        public StarRule(Rule x)
        {
            AddRule(x);
        }

        public override string RuleDefinition
        {
            get { return Rules[0] + "*"; } 
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            Trace.Assert(Rules.Count == 1);
            Rule r = Rules[0];
            while (true)
            {
                int old = p.Index;
                if (!r.Match(p))
                    return true;

                // Avoid infinite loops.
                if (p.Index <= old)
                    throw new Exception("Failed to advance parser input pointer, while parsing rule '" + 
                        r.RuleNameOrDefinition + "'. This means the grammar is invalid, maybe because of nested star rules.");
            }
        }
    }

    /// <summary>
    /// Matches a sub-rule 1 or more times.
    /// </summary>
    public class PlusRule : Rule
    {
        public override string RuleType
        {
            get { return "plus"; }
        }

        public PlusRule(Rule x)
        {
            AddRule(x);
        }

        public override string RuleDefinition
        {
            get { return Rules[0] + "+"; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            Trace.Assert(Rules.Count == 1);
            Rule r = Rules[0];
            if (!r.Match(p))
                return false;
            while (true)
            {
                int old = p.Index;
                if (!r.Match(p))
                    return true;

                // Avoid infinite loops.
                if (p.Index <= old)
                    throw new Exception("Failed to advance parser input pointer, while parsing rule '" +
                        r.RuleNameOrDefinition + "'. This means the grammar is invalid, maybe because of nested star rules.");
            }
        }
    }

    /// <summary>
    /// Returns true if the sub-rule is not successful, or false otherwise.
    /// </summary>
    public class NotRule : Rule
    {
        public NotRule(Rule x)
        {
            AddRule(x);
        }

        public override string RuleType
        {
            get { return "not"; }
        }

        public override string RuleDefinition
        {
            get { return Rules[0] + "^"; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            Trace.Assert(Rules.Count == 1);
            Rule r = Rules[0];
            if (r.Match(p))
                return false;
            return true;
        }
    }

    /// <summary>
    /// Returns true, whether or not the sub-rule is not successful.
    /// </summary>
    public class OptRule : Rule
    {
        public OptRule(Rule x)
        {
            AddRule(x);
        }

        public override string RuleType
        {
            get { return "opt"; }
        }

        public override string RuleDefinition
        {
            get { return Rules[0] + "?"; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            Rule r = Rules[0];
            r.Match(p);
            return true;
        }
    }

    /// <summary>
    /// Evaluates a function at run-time, to perform the matching for it.
    /// Used to allow circular references in the grammar.
    /// </summary>
    public class RecursiveRule : Rule
    {
        Func<Rule> func;

        public RecursiveRule(Func<Rule> f)
        {
            func = f;
        }

        public override string RuleType
        {
            get { return "recursive"; } 
        }

        public override string RuleDefinition
        {
            get
            {
                return "recursive"; 
            }
        }

        protected override bool InternalMatch(ParserState p)
        {
            Rule r = func();
            return r.Match(p);
        }
    }

    /// <summary>
    /// Matches a sequence of characters in the input text
    /// </summary>
    public class CharSeqRule : Rule
    {
        private readonly string str;
        public CharSeqRule(string s)
        {
            str = s ?? throw new ArgumentNullException(nameof(s));
            Trace.Assert(s.Length > 0);
        }

        public override string RuleType
        {
            get { return "charsequence"; }
        }

        public override string RuleDefinition
        {
            get { return "[" + str + "]"; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            int n = str.Length;
            if (p.Index + n > p.Text.Length)
                return false;
            for (int i = 0; i < n; ++i)
            {
                if (p.Text[p.Index++] != str[i])
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Matches a single character against a set of characters in the input text.
    /// </summary>
    public class CharSetRule : Rule
    {
        private readonly string str;
        public CharSetRule(string s)
        {
            str = s;
        }

        public override string RuleType
        {
            get { return "charset"; }
        }

        public override string RuleDefinition
        {
            get { return "[" + str + "]"; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            if (p.AtEndOfInput())
                return false;

            char c = p.Text[p.Index++];
            return str.Contains(c);
        }
    }

    /// <summary>
    /// Matches a single character in the input text against a range of valid characters
    /// </summary>
    public class CharRangeRule : Rule
    {
        private readonly char first;
        private readonly char last;

        public CharRangeRule(char from, char to)
        {
            first = from;
            last = to;
        }

        public override string RuleType
        {
            get { return "charrange"; }
        }

        public override string RuleDefinition
        {
            get { return "[" + first + ".." + last + "]"; } 
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            if (p.AtEndOfInput())
                return false;

            char c = p.Text[p.Index++];
            return (c >= first && c <= last);
        }
    }

    /// <summary>
    /// Throws an exception if parsing fails, instead of returning false
    /// </summary>
    public class NoFailRule : Rule
    {
        public NoFailRule(Rule x)
        {
            AddRule(x);
        }

        public override string RuleType
        {
            get { return "nofail"; }
        }

        public override string RuleDefinition
        {
            get { return Rules[0].RuleDefinition; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            Rule r = Rules[0];
            if (!r.Match(p))
            {
                ParsingException ex = new ParsingException(p.Peek(), r, p);
                throw ex;
            }
            return true;
        }
    }

    /// <summary>
    /// Used to prevent creation of parse node in the AST tree. For 
    /// example whitespace. 
    /// </summary>
    public class SkipRule : Rule
    {
        public SkipRule(Rule x)
        {
            AddRule(x);
        }

        public override string RuleDefinition
        {
            get { return "skip(" + Rules[0].RuleDefinition + ")"; }
        }

        public override string RuleType
        {
            get { return "skip"; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            // Tell the parser state to stop creating nodes. 
            bool store = p.CreateNodes;
            p.CreateNodes = false;
            bool result = false;
            try
            {
                result = Rules[0].Match(p);
            }
            finally
            {
                p.CreateNodes = store;
            }
            return result;
        }
    }

    /// <summary>
    /// Used to prevent creation of child parse nodes in the AST tree. For 
    /// example identifiers. Similar to SkipRule.
    /// </summary>
    public class LeafRule : Rule
    {
        public LeafRule(Rule x)
        {
            AddRule(x);
        }

        public override string RuleDefinition
        {
            get { return Rules[0].RuleDefinition; }
        }

        public override string RuleType
        {
            get { return "leaf"; }
        }

        protected override bool InternalMatch(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            // Tell the parser state to stop creating nodes. 
            bool store = p.CreateNodes;
            p.CreateNodes = false;
            bool result = false;
            try
            {
                result = Rules[0].Match(p);
            }
            finally
            {
                p.CreateNodes = store;
            }
            return result;
        }
    }
    /// <summary>
    /// A rule that returns true if at the end of input.
    /// </summary>
    public class EndOfInputRule : Rule
    {
        public override string RuleDefinition
        {
            get { return "_EOF_"; }
        }

        public override string RuleType
        {
            get { return "_EOF_"; }
        }

        public override bool Match(ParserState p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            return p.AtEndOfInput();
        }

        protected override bool InternalMatch(ParserState p)
        {
            throw new Exception("Error: EndOfInput.InternalMatch() should never be called");
        }
    }
}
