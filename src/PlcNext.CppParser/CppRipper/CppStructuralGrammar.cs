
using System.Reflection;

namespace PlcNext.CppParser.CppRipper
{
    /// <summary>
    /// This grammar is used for parsing C/C++ files into declarations, including comments.
    /// The goal is to get a high-level structure of the language, and understand 
    /// where the comments relate to items.
    /// </summary>
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class CppStructuralGrammar
        : CppBaseGrammar
    {
        public Rule paran_group;
        public Rule brace_group;
        public Rule bracketed_group;
        public Rule symbol;
        public Rule template_decl;
        public Rule typedef_decl;
        public Rule class_decl;
        public Rule struct_decl;
        public Rule enum_decl;
        public Rule union_decl;
        public Rule type_decl;
        public Rule pp_directive;
        public Rule declaration_content;
        public Rule node;
        public Rule declaration;
        public Rule declaration_list;
        public Rule same_line_comment;
        public Rule comment_set;
        public Rule label;
        public Rule file;
        public Rule visibility;
        public Rule visibility_group;
        public Rule base_type;
        public Rule angle_operators;
        public Rule identifier_seq;
        public Rule generic;

        public Rule Delimiter(string s)
        {
            return Skip(CharSeq(s)) + Eat(multiline_ws);
        }

        public CppStructuralGrammar()
        {
            generic = (identifier + Delimiter("<") + 
                      Recursive(() => CommaList(Eat(simple_ws)+node+Eat(simple_ws))) + 
                      Delimiter(">") + Opt(identifier_extension)) |
                      (identifier + Delimiter("<") + Delimiter(">"));
            
            comment_set
                = Star(comment + Eat(multiline_ws)) + Eat(multiline_ws);

            same_line_comment
                = Eat(simple_ws) + comment;

            declaration_list
                = Recursive(() => Star(declaration) + comment_set);

            bracketed_group
                = Delimiter("[") + declaration_list + NoFail(Delimiter("]"));

            paran_group
                = Delimiter("(") + declaration_list + NoFail(Delimiter(")"));

            brace_group
                = Delimiter("{") + declaration_list + NoFail(Delimiter("}"));

            symbol
                = Not(CharSeq("/*") | CharSeq("//")) + CharSet("~!@%^&*-+=|:.?/,") + Eat(multiline_ws);

            template_decl
                = TEMPLATE + ((Delimiter("<") + Plus(TYPENAME + identifier + Opt(COMMA) + Eat(multiline_ws)) + Delimiter(">"))
                              | Nested("<",">"))
                + ws;

            typedef_decl
                = TYPEDEF + Eat(multiline_ws);

            visibility = PRIVATE | PUBLIC | PROTECTED;

            base_type = Opt(visibility) + Eat(multiline_ws) + (generic | identifier);

            class_decl
                = CLASS + Opt((generic | identifier) + Opt(Eat(multiline_ws) + COLON +
                                               Plus(Eat(multiline_ws) + base_type + Eat(multiline_ws) + Opt(COMMA)))) + Eat(multiline_ws);

            struct_decl
                = STRUCT + Opt((generic | identifier) + Opt(Eat(multiline_ws) + COLON +
                                               Plus(Eat(multiline_ws) + base_type + Eat(multiline_ws) + Opt(COMMA)))) + Eat(multiline_ws);

            union_decl
                = UNION + Opt(identifier) + Eat(multiline_ws);

            enum_decl
                = ENUM + Opt(CLASS|STRUCT) + Opt(identifier + Opt(Eat(multiline_ws) + COLON +
                                              Eat(multiline_ws) + base_type + Eat(multiline_ws) +
                                               Star(COMMA + Eat(multiline_ws) + base_type + Eat(multiline_ws)))) + Eat(multiline_ws);

            label
                = identifier + ws + COLON + Eat(multiline_ws);

            visibility_group = visibility + Eat(multiline_ws) + COLON + Eat(multiline_ws);

            pp_directive
                = CharSeq("#") + NoFailSeq(ws + identifier + Eat(simple_ws) + until_eol + (eol | EndOfInput()));

            type_decl 
                = Opt(template_decl) + (class_decl | struct_decl | union_decl | enum_decl);

            angle_operators
                = Not(CharSeq("/*") | CharSeq("//")) + (CharSeq("<<=") | CharSeq(">>=") | CharSeq("<<") | CharSeq(">>") | CharSeq("<=") | CharSeq(">=") | CharSeq("<") | CharSeq(">")) + Eat(multiline_ws);

            identifier_seq
                = (generic | identifier) + Plus(CharSeq("->") + (generic | identifier));

            node 
                = bracketed_group
                | paran_group
                | brace_group
                | type_decl
                | typedef_decl
                | template_decl
                | literal
                | symbol
                | @operator
                | visibility_group
                | label
                | identifier_seq
                | generic
                | identifier
                | angle_operators;

            declaration_content
                = Plus(comment_set + node + Eat(multiline_ws));

            declaration
                = comment_set + pp_directive + Eat(multiline_ws)
                | comment_set + semicolon + Opt(same_line_comment) + Eat(multiline_ws)
                | declaration_content + Opt(semicolon) + Opt(same_line_comment) + Eat(multiline_ws);

            file
                = Opt(comment_set) + declaration_list + ws + NoFail(EndOfInput());

            //===============================================================================================
            // Tidy up the grammar, and assign rule names from the field names.

            InitializeRules<CppStructuralGrammar>();
        }
    }
}
