
using System.CodeDom.Compiler;
using System.Reflection;

namespace PlcNext.CppParser.CppRipper
{
    /// <summary>
    /// A grammar for some of the basic syntactic elements for the C++ language
    /// Inspired by:
    /// * http://www.lysator.liu.se/c/ANSI-C-grammar-y.html
    /// * http://cpp.comsci.us/etymology/literals.html   
    /// </summary>
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    [GeneratedCode("CppRipper","1.0")]
    public class CppBaseGrammar
        : BaseGrammar
    {
        #region keyword rules
        public Rule COND;
        public Rule DOT;
        public Rule COLON;
        public Rule AMP;
        public Rule PLUS;
        public Rule MINUS;
        public Rule STAR;
        public Rule SLASH;
        public Rule MOD;
        public Rule NOT;
        public Rule TILDE;
        public Rule CARET;
        public Rule PIPE;
        public Rule EQ;
        public Rule COMMA;
        public Rule SIZEOF;
        public Rule PTR_OP;
        public Rule INC_OP;
        public Rule DEC_OP;
        public Rule LEFT_OP;
        public Rule RIGHT_OP;
        public Rule LT_OP;
        public Rule GT_OP;
        public Rule LE_OP;
        public Rule GE_OP;
        public Rule EQ_OP;
        public Rule NE_OP;
        public Rule AND_OP;
        public Rule OR_OP;
        public Rule MUL_ASSIGN;
        public Rule DIV_ASSIGN;
        public Rule MOD_ASSIGN;
        public Rule ADD_ASSIGN;
        public Rule SUB_ASSIGN;
        public Rule LEFT_ASSIGN;
        public Rule RIGHT_ASSIGN;
        public Rule AND_ASSIGN;
        public Rule XOR_ASSIGN;
        public Rule OR_ASSIGN;
        public Rule TYPEDEF;
        public Rule EXTERN;
        public Rule STATIC;
        public Rule AUTO;
        public Rule REGISTER;
        public Rule CHAR;
        public Rule SHORT;
        public Rule INT;
        public Rule LONG;
        public Rule SIGNED;
        public Rule UNSIGNED;
        public Rule FLOAT;
        public Rule DOUBLE;
        public Rule CONST;
        public Rule VOLATILE;
        public Rule VOID;
        public Rule STRUCT;
        public Rule UNION;
        public Rule ENUM;
        public Rule ELLIPSIS;
        public Rule CASE;
        public Rule DEFAULT;
        public Rule IF;
        public Rule ELSE;
        public Rule SWITCH;
        public Rule WHILE;
        public Rule DO;
        public Rule FOR;
        public Rule GOTO;
        public Rule CONTINUE;
        public Rule BREAK;
        public Rule RETURN;
        public Rule CLASS;
        public Rule TYPENAME;
        public Rule TYPEID;
        public Rule TEMPLATE;
        public Rule PUBLIC;
        public Rule PROTECTED;
        public Rule PRIVATE;
        public Rule VIRTUAL;
        public Rule OPERATOR;
        public Rule USING;
        public Rule BUILTIN_LITERAL_SUFFIXES;
        #endregion

        #region identifiers
        public Rule digit;
        public Rule lower_case_letter;
        public Rule upper_case_letter;
        public Rule letter;
        public Rule ident_first_char;
        public Rule ident_next_char;
        public Rule identifier_extension;
        public Rule identifier;
        public Rule operator_char;
        public Rule operator_prefix;
        public Rule operator_seq;
        public Rule @operator;
        #endregion

        #region numbers
        public Rule octal_digit;
        public Rule nonzero_digit;
        public Rule hex_digit;
        public Rule sign;
        #endregion numbers

        #region whitespace
        public Rule comment;
        public Rule line_comment_content;
        public Rule full_comment_content;
        public Rule line_comment;
        public Rule full_comment;
        public Rule tab;
        public Rule space;
        public Rule ext_line;
        public Rule semicolon;  
        public Rule eos;
        public Rule eol;
        public Rule until_eol;
        public Rule multiline_ws;
        public Rule simple_ws;
        public Rule ws;
        #endregion

        #region literals
        public Rule dbl_quote;
        public Rule quote;
        public Rule dot;
        public Rule literal;
        public Rule int_literal;
        public Rule custom_literal;
        public Rule unsigned_literal;
        public Rule decimal_literal;
        public Rule octal_literal;
        public Rule hex_literal;
        public Rule hex_prefix;
        public Rule integer_suffix;
        public Rule long_suffix;
        public Rule unsigned_suffix;
        public Rule float_literal;
        public Rule unsigned_float;
        public Rule simple_float;
        public Rule exponential_float;
        public Rule exponent_part;
        public Rule exponent_prefix;
        public Rule exponent;
        public Rule digit_sequence;
        public Rule float_suffix;
        public Rule char_literal;
        public Rule c_char;
        public Rule s_char;
        public Rule string_literal;
        public Rule escape_sequence;
        public Rule simple_escape;
        public Rule octal_escape;
        public Rule hex_escape;
        public Rule boolean_literal;
        public Rule user_literal_suffix;
        #endregion

        #region pre-processor directives
        public Rule pragma;
        public Rule included_file;
        public Rule include;
        public Rule ifdef_macro;
        public Rule endif_macro;
        public Rule elif_macro;
        public Rule else_macro;
        #endregion

        #region rule generating functions

        /// <summary>
        /// Creates a rule that matches the rule R multiple times, delimited by commas. 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public Rule CommaList(Rule r)
        {
            return r + Star(COMMA + r);
        }

        /// <summary>
        /// Creates a rule that matches a pair of rules, consuming all nested pairs within
        /// as well. 
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Rule Nested(Rule begin, Rule end)
        {
            RecursiveRule recursive = new RecursiveRule(() => { return Nested(begin, end); });
            return begin + NoFailSeq(Star(recursive | (Not(end) + Not(begin) + Anything())) + end);
        }

        /// <summary>
        /// Creates a rule that matches a pair of character sequences, consuming all nested pairs 
        /// within as well. For example: "Nested("{", "}"));
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Rule Nested(string begin, string end)
        {
            return Nested(CharSeq(begin), CharSeq(end));
        }

        /// <summary>
        /// Creates a SkipRule that matches a sequence of characters, like CharSeq. It also assures that no other letters follows 
        /// and will also eats whitespace. No node is created.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Rule Word(string s) 
        {
            return CharSeq(s) + Not(ident_next_char) + Eat(multiline_ws);
        }
        #endregion

        /// <summary>
        /// Constructor: initializes the public rule fields.
        /// </summary>
        public CppBaseGrammar()
        {
            #region numbers
            digit = CharRange('0', '9');
            octal_digit = CharRange('0', '7');
            nonzero_digit = CharRange('1', '9');
            hex_digit = digit | CharRange('a', 'f') | CharRange('A', 'F');
            sign = CharSet("+-");
            #endregion numbers
            
            #region identifiers
            lower_case_letter = CharRange('a', 'z');
            upper_case_letter = CharRange('A', 'Z');
            letter = lower_case_letter | upper_case_letter;
            ident_first_char = CharSet("_") | letter;
            ident_next_char = ident_first_char | digit;
            identifier_extension = CharSeq("::") + Recursive(() => Opt(identifier));
            identifier = Leaf(ident_first_char + Star(ident_next_char) + Star(identifier_extension));
            
            #endregion

            #region whitespace
            tab = CharSeq("\t");
            space = CharSeq(" ");
            simple_ws = tab | space;
            eol = Opt(CharSeq("\r")) + CharSeq("\n");
            ext_line = CharSeq("\\") + Star(simple_ws) + eol;
            multiline_ws = simple_ws | eol;
            until_eol = Star(ext_line | AnythingBut(eol | EndOfInput()));
            line_comment_content = until_eol;
            line_comment = CharSeq("//") + NoFailSeq(line_comment_content + (eol | EndOfInput()));
            full_comment_content = Until(CharSeq("*/"));
            full_comment = CharSeq("/*") + NoFailSeq(full_comment_content + CharSeq("*/"));
            comment = line_comment | full_comment;
            ws = Eat(multiline_ws | comment);
            #endregion

            #region keyword rules
            COND = Word("?");
            DOT = Word(".");
            COLON = Word(":");
            AMP = Word("&");
            PLUS = Word("+");
            MINUS = Word("-");
            STAR = Word("*");
            SLASH = Word("/");
            MOD = Word("%");
            NOT = Word("!");
            TILDE = Word("~");
            CARET = Word("^");
            PIPE = Word("|");
            EQ = Word("=");
            COMMA = CharSeq(",");
            SIZEOF = Word("sizeof");
            PTR_OP = Word("->");
            INC_OP = Word("++");
            DEC_OP = Word("--");
            LEFT_OP = Word("<<");
            RIGHT_OP = Word(">>");
            LT_OP = Word("<");
            GT_OP = Word(">");
            LE_OP = Word("<=");
            GE_OP = Word(">=");
            EQ_OP = Word("==");
            NE_OP = Word("!=");
            AND_OP = Word("&&");
            OR_OP = Word("||");
            MUL_ASSIGN = Word("*=");
            DIV_ASSIGN = Word("/=");
            MOD_ASSIGN = Word("%=");
            ADD_ASSIGN = Word("+=");
            SUB_ASSIGN = Word("-=");
            LEFT_ASSIGN = Word("<<=");
            RIGHT_ASSIGN = Word(">>=");
            AND_ASSIGN = Word("&=");
            XOR_ASSIGN = Word("^=");
            OR_ASSIGN = Word("|=");
            TYPEDEF = Word("typedef");
            EXTERN = Word("extern");
            STATIC = Word("static");
            AUTO = Word("auto");
            REGISTER = Word("register");
            CHAR = Word("char");
            SHORT = Word("short");
            INT = Word("int");
            LONG = Word("long");
            SIGNED = Word("signed");
            UNSIGNED = Word("unsigned");
            FLOAT = Word("float");
            DOUBLE = Word("double");
            CONST = Word("const");
            VOLATILE = Word("volatile");
            VOID = Word("void");
            STRUCT = Word("struct");
            UNION = Word("union");
            ENUM = Word("enum");
            ELLIPSIS = Word("...");
            CASE = Word("case");
            DEFAULT = Word("default");
            IF = Word("if");
            ELSE = Word("else");
            SWITCH = Word("switch");
            WHILE = Word("while");
            DO = Word("do");
            FOR = Word("for");
            GOTO = Word("goto");
            CONTINUE = Word("continue");
            BREAK = Word("break");
            RETURN = Word("return");
            CLASS = Word("class");
            TYPENAME = Word("typename");
            TYPEID = Word("typeid");
            TEMPLATE = Word("template");
            PUBLIC = Word("public");
            PROTECTED = Word("protected");
            PRIVATE = Word("private");
            VIRTUAL = Word("virtual");
            OPERATOR = Word("operator");
            USING = Word("using");
            BUILTIN_LITERAL_SUFFIXES = Word("if")
                                       | Word("i")
                                       | Word("il")
                                       | Word("h")
                                       | Word("min")
                                       | Word("s")
                                       | Word("ms")
                                       | Word("us")
                                       | Word("ns")
                                       | Word("y")
                                       | Word("d")
                                       | Word("s")
                                       | Word("sv");
            #endregion

            #region literals
            dot
                = CharSeq(".");
            dbl_quote 
                = CharSeq("\"");
            quote 
                = CharSeq("\'");
            simple_escape
                = CharSeq("\\") + CharSet("abfnrtv'\"?\\");
            octal_escape
                = CharSeq("\\") + octal_digit + Opt(octal_digit + Opt(octal_digit));
            hex_escape
                = CharSeq("\\x") + Star(hex_digit);
            escape_sequence
                = simple_escape
                | octal_escape
                | hex_escape;
            c_char
                = escape_sequence | Not(quote) + Anything();
            s_char
                = escape_sequence | Not(dbl_quote) + Anything();
            long_suffix	
                = CharSet("Ll");
            unsigned_suffix	
                = CharSet("Uu");
            digit_sequence	 
                = Plus(digit);
            exponent
                = Opt(sign) + digit_sequence;
            exponent_prefix
                = CharSet("Ee");
            exponent_part
                = exponent_prefix + exponent;
            float_suffix
                = CharSet("LlFf");
            simple_float
                = CharSeq(".") + digit_sequence 
                | digit_sequence + dot + Opt(digit_sequence);
            exponential_float
                = digit_sequence + exponent_part
                | simple_float + exponent_part;
            unsigned_float
                = exponential_float
                | simple_float;
            hex_prefix
                = CharSeq("0X") | CharSeq("0x");
            hex_literal
                = hex_prefix + Plus(hex_digit);
            octal_literal
                = CharSeq("0") + Star(octal_digit);
            decimal_literal
                = nonzero_digit + Star(digit);
            unsigned_literal
                = hex_literal
                  | octal_literal
                  | decimal_literal;
            integer_suffix
                = unsigned_suffix + Opt(long_suffix) + Opt(long_suffix)
                  | long_suffix + Opt(long_suffix) + Opt(unsigned_suffix);
            int_literal
                = unsigned_literal + Not(dot) + Not(exponent_prefix) + Opt(integer_suffix);
            float_literal
                = unsigned_float + Opt(float_suffix);
            char_literal
                = Opt(CharSeq("L")) + quote + Star(c_char) + quote;
            string_literal
                = Opt(CharSeq("L")) + dbl_quote + Star(s_char) + dbl_quote;
            boolean_literal
                = Word("true") | Word("false");
            user_literal_suffix
                = CharSeq("_") + identifier;
            custom_literal
                = (int_literal | float_literal) + (BUILTIN_LITERAL_SUFFIXES | user_literal_suffix);
            literal
                = (custom_literal 
                   | int_literal
                   | char_literal
                   | float_literal
                   | string_literal
                   | boolean_literal)
                + NoFail(Not(ident_next_char)) + ws;
            #endregion

            #region pre-processor directives
            pragma = Word("#") + Word("pragma") + until_eol;
            included_file = string_literal | CharSeq("<") + Star(Not(CharSeq(">")) + Anything()) + CharSeq(">");
            include = Word("#") + Word("include") + included_file;
            ifdef_macro = Word("#") + Word("if") + until_eol + eol;
            endif_macro = Word("#") + Word("endif") + until_eol + eol;
            elif_macro = Word("#") + Word("elif") + until_eol + eol;
            else_macro = Word("#") + Word("else") + until_eol + eol;
            #endregion

            #region symbols
            semicolon = CharSeq(";");
            eos = Word(";");
            #endregion

            #region operators

            operator_char = CharSet("=<>!-+*/[]%&|^~?");
            operator_prefix = Leaf(Star(ident_first_char + Star(ident_next_char) + CharSeq("::")));
            operator_seq = Leaf(operator_char + Opt(operator_char));
            @operator = operator_prefix + OPERATOR + operator_seq;

            #endregion

            InitializeRules<CppBaseGrammar>();
        }
    }
}
