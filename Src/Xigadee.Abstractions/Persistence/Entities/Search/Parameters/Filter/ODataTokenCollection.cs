using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class splits an OData filter string in to a series of tokens.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class ODataTokenCollection
    {
        #region Constructors
        /// <summary>
        /// This is the constructor that appends a default string.
        /// </summary>
        /// <param name="filter">The filter string.</param>
        public ODataTokenCollection(string filter)
        {
            OriginalFilter = filter;

            filter.ForEach(c => Write(c));

            ExpressionTree = new ODataExpressionTree(TokensWithoutSeperators);
        } 
        #endregion

        /// <summary>
        /// This is the expression tree that holds the ordered collection.
        /// </summary>
        protected ODataExpressionTree ExpressionTree { get; }
        /// <summary>
        /// This is the original filter.
        /// </summary>
        public string OriginalFilter { get; }

        /// <summary>
        /// This is the list of token.
        /// </summary>
        public List<ODataTokenBase> Tokens = new List<ODataTokenBase>();
        /// <summary>
        /// This is the list of tokens without seperators. 
        /// </summary>
        public List<ODataTokenBase> TokensWithoutSeperators => Tokens.Where(t => !(t is ODataTokenSeperator)).ToList();
        /// <summary>
        /// This is the first token in the chain.
        /// </summary>
        public ODataTokenBase First => Tokens.Count > 0 ? Tokens[0] : null;

        /// <summary>
        /// This is the current token that can be written to.
        /// </summary>
        public ODataTokenBase Current => Tokens.Count > 0 ? Tokens[Tokens.Count-1] : null;

        /// <summary>
        /// The collection string length.
        /// </summary>
        public int Length => Tokens.Count == 0 ? 0 : Tokens.Sum(t => t.Length);
        /// <summary>
        /// This writes a character to the tokenizer.
        /// </summary>
        /// <param name="next">The character to write.</param>
        public void Write(char next)
        {
            //Keep writing until the token says it cannot accept any more.
            if (Current != null && Current.Write(next))
                return;

            //OK, we need to create a new token
            if (ODataTokenSeperator.CanStart(next))
                Tokens.Add(new ODataTokenSeperator(next));
            //Open bracket
            else if (ODataTokenBracketOpen.CanStart(next))
                Tokens.Add(new ODataTokenBracketOpen(next));
            //Close bracket
            else if (ODataTokenBracketClose.CanStart(next))
                Tokens.Add(new ODataTokenBracketClose(next));
            //Quoted string
            else if (ODataTokenQuotedString.CanStart(next))
                Tokens.Add(new ODataTokenQuotedString(next));
            //Token
            else if (ODataTokenString.CanStart(next))
                Tokens.Add(new ODataTokenString(next));
            else
                throw new ArgumentOutOfRangeException($"Invalid character '{next}' at position {Length}");
        }

        /// <summary>
        /// This is the default override to convert the tokens back in to a string.
        /// </summary>
        /// <returns>Returns the Select statement.</returns>
        public override string ToString() => ToString(false);
        /// <summary>
        /// This converts the tokens back in to a string.
        /// </summary>
        /// <returns>Returns the Select statement.</returns>
        public virtual string ToString(bool removeBrackets)
        {
            var sb = new StringBuilder();

            for(int i = 0;i< Tokens.Count; i++)
            {
                var token = Tokens[i];

                if (token is ODataTokenSeperator)
                {
                    //Are we the first or last token
                    if (i == 0 || i == (Tokens.Count - 1))
                        continue;

                    //Is the previous token a bracket open, then skip
                    if (!removeBrackets && Tokens[i - 1] is ODataTokenBracketOpen)
                        continue;

                    //Is the next token a brackets close, then skip
                    if ((!removeBrackets && Tokens[i +1] is ODataTokenBracketClose))
                        continue;

                    //We only write standard whitespace.
                    sb.Append(" ");
                    continue;
                }

                if (removeBrackets && token is ODataTokenBracketBase)
                {
                    //sb.Append(" ");
                    continue;
                }

                token.Append(sb);
            }

            return sb.ToString();
        }

        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params => ExpressionTree?.Params;

        /// <summary>
        /// This is a list of valid solutions for the logical collection.
        /// </summary>
        public List<int> Solutions => ExpressionTree?.Solutions;
    }

    #region ODataTokenString
    /// <summary>
    /// This is the string token that supports commands and logical operators.
    /// </summary>
    public class ODataTokenString : ODataTokenStringBase
    {
        #region OData constants
        /// <summary>
        /// This method converts the shortcut enumeration to the actual string representation.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <returns>Returns the string equivalent.</returns>
        public static string Convert(ODataFilterOperations op)
        {
            switch (op)
            {
                case ODataFilterOperations.Equal:
                    return ODataEqual;
                case ODataFilterOperations.NotEqual:
                    return ODataNotEqual;
                case ODataFilterOperations.LessThan:
                    return ODataLessThan;
                case ODataFilterOperations.LessThanOrEqual:
                    return ODataLessThanOrEqual;
                case ODataFilterOperations.GreaterThan:
                    return ODataGreaterThan;
                case ODataFilterOperations.GreaterThanOrEqual:
                    return ODataGreaterThanOrEqual;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Null
        /// </summary>
        public const string ODataNull = "null";

        /// <summary>
        /// Not
        /// </summary>
        public const string ODataNot = "not";

        /// <summary>
        /// Equal
        /// </summary>
        public const string ODataEqual = "eq";
        /// <summary>
        /// Not equal
        /// </summary>
        public const string ODataNotEqual = "ne";
        /// <summary>
        /// Less than
        /// </summary>
        public const string ODataLessThan = "lt";
        /// <summary>
        /// Less than or equal
        /// </summary>
        public const string ODataLessThanOrEqual = "le";
        /// <summary>
        /// Greater than
        /// </summary>
        public const string ODataGreaterThan = "gt";
        /// <summary>
        /// Greater than or equal
        /// </summary>
        public const string ODataGreaterThanOrEqual = "ge";

        /// <summary>
        /// And, Or, XOr
        /// </summary>
        public static IReadOnlyList<string> ODataExpression => new[] { ODataEqual, ODataNotEqual, ODataLessThan, ODataLessThanOrEqual, ODataGreaterThan, ODataGreaterThanOrEqual };

        #endregion
        #region Static declarations: Conditionals
        /// <summary>
        /// This method converts the shortcut enumeration to the actual string representation.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <returns>Returns the string equivalent.</returns>
        public static ODataLogicalOperators Convert(string op)
        {
            switch (op?.Trim().ToLowerInvariant() ?? "")
            {
                case ODataConditionalAnd:
                    return ODataLogicalOperators.OpAnd;
                case ODataConditionalOr:
                    return ODataLogicalOperators.OpOr;
                case ODataConditionalXOr:
                    return ODataLogicalOperators.OpXor;
                default:
                    throw new ArgumentOutOfRangeException(op);
            }
        }

        /// <summary>
        /// This method converts the shortcut enumeration to the actual string representation.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <returns>Returns the string equivalent.</returns>
        public static string Convert(ODataLogicalOperators op)
        {
            switch (op)
            {
                case ODataLogicalOperators.OpAnd:
                    return ODataConditionalAnd;
                case ODataLogicalOperators.OpOr:
                    return ODataConditionalOr;
                case ODataLogicalOperators.OpXor:
                    return ODataConditionalXOr;
                default:
                    throw new ArgumentOutOfRangeException(op.ToString());
            }
        }
        /// <summary>
        /// And
        /// </summary>
        public const string ODataConditionalAnd = "and";
        /// <summary>
        /// Or
        /// </summary>
        public const string ODataConditionalOr = "or";

        /// <summary>
        /// XOr
        /// </summary>
        public const string ODataConditionalXOr = "xor";
        /// <summary>
        /// And, Or, XOr
        /// </summary>
        public static IReadOnlyList<string> ODataConditionals => new[] { ODataConditionalAnd, ODataConditionalOr, ODataConditionalXOr };
        #endregion

        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="c">The incoming character.</param>
        public ODataTokenString(char c) : base(c)
        {
        }
        #endregion

        /// <summary>
        /// Specifies that the token can start if it is a character or digit. We accept unicode character as well.
        /// We also accept the underscore character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Returns true if it can start.</returns>
        public static bool CanStart(char c)
        {
            return char.IsLetterOrDigit(c) || c =='_';
        }

        /// <summary>
        /// The token is numeric. This includes numbers with decimal places.
        /// </summary>
        public bool IsNumeric => decimal.TryParse(ToString(), out var dontcare);

        /// <summary>
        /// This is a known logical command, and, or xor.
        /// </summary>
        public bool IsCommandLogical => ODataConditionals.Contains(ToString().ToLowerInvariant());

        /// <summary>
        /// This is a known expression, i.e. eq, ne, gt etc.
        /// </summary>
        public bool IsCommandExpression => ODataExpression.Contains(ToString().ToLowerInvariant());

        /// <summary>
        /// This is a known command
        /// </summary>
        public bool IsCommand => IsCommandLogical || IsCommandExpression;

        /// <summary>
        /// Null
        /// </summary>
        public bool IsKeywordNull => string.Equals(ODataNull, ToString());

        /// <summary>
        /// Not
        /// </summary>
        public bool IsKeywordNot => string.Equals(ODataNot, ToString());

        /// <summary>
        /// The token is a key word.
        /// </summary>
        public bool IsKeyword => IsKeywordNull || IsKeywordNot;

        /// <summary>
        /// We allow a decimal place to be appended if it is not the first character, there is not already a decimal place,
        /// and the token is numeric.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>Returns true if supported.</returns>
        protected override bool Supports(char c)
        {
            if (c == '.')
                return (!_sb.ContainsChar('.') && int.TryParse(_sb.ToString(), out var dontcare));

            return CanStart(c);
        }
    } 
    #endregion
    #region ODataTokenQuotedString
    /// <summary>
    /// This token supports a text string.
    /// </summary>
    public class ODataTokenQuotedString : ODataTokenStringBase
    {
        bool? _endReached = null;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="c">The incoming character.</param>
        public ODataTokenQuotedString(char c) : base(c)
        {
        }

        /// <summary>
        /// Is it a known seperator?
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Returns true if matched.</returns>
        public static bool CanStart(char c)
        {
            return char.Equals(c, '\'');
        }

        /// <summary>
        /// We only allow a single character.
        /// </summary>
        /// <param name="c">The character to write.</param>
        /// <returns>Returns false.</returns>
        public override bool Write(char c)
        {
            if (_endReached ?? false)
                return false;

            if (!_endReached.HasValue)
                _endReached = false;
            else
                _endReached = char.Equals(c, '\'');

            WriteInternal(c);
            return true;
        }

        /// <summary>
        /// This class supports anything until the final ' character has been reached.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override bool Supports(char c) => true;

    } 
    #endregion
    #region ODataTokenSeperator
    /// <summary>
    /// This is the seperator tolen.
    /// </summary>
    public class ODataTokenSeperator : ODataTokenStringBase
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="c">The incoming character.</param>
        public ODataTokenSeperator(char c) : base(c)
        {
        }

        /// <summary>
        /// Is it a known seperator?
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Returns true if matched.</returns>
        public static bool CanStart(char c)
        {
            return char.IsWhiteSpace(c) || char.IsSeparator(c);
        }

        /// <summary>
        /// Is it a known seperator?
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Returns true if matched.</returns>
        protected override bool Supports(char c) => CanStart(c);
    } 
    #endregion
    #region ODataTokenBracketOpen
    /// <summary>
    /// This is the open bracket token.
    /// </summary>
    public class ODataTokenBracketOpen : ODataTokenBracketBase
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="c">The incoming character.</param>
        public ODataTokenBracketOpen(char c) : base('(')
        {
        }

        /// <summary>
        /// Is it an open bracket.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Returns true if matched.</returns>

        public static bool CanStart(char c)
        {
            return char.Equals(c, '(');
        }
    } 
    #endregion
    #region ODataTokenBracketClose
    /// <summary>
    /// This is the close bracket token.
    /// </summary>
    public class ODataTokenBracketClose : ODataTokenBracketBase
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="c">The incoming character.</param>
        public ODataTokenBracketClose(char c) : base(')')
        {
        }

        /// <summary>
        /// Is it an close bracket.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Returns true if matched.</returns>
        public static bool CanStart(char c)
        {
            return char.Equals(c, ')');
        }
    } 
    #endregion
    #region ODataTokenBracketBase
    /// <summary>
    /// This is the token base for brackets.
    /// </summary>
    public abstract class ODataTokenBracketBase : ODataTokenBase
    {
        char _comp;

        /// <summary>
        /// This is the base constructor.
        /// </summary>
        /// <param name="comp">The bracket character.</param>
        protected ODataTokenBracketBase(char comp)
        {
            _comp = comp;
        }

        /// <summary>
        /// We only allow a single character.
        /// </summary>
        /// <param name="c">The character to write.</param>
        /// <returns>Returns false.</returns>
        public override bool Write(char c) => false;


        /// <summary>
        /// This is always 1.
        /// </summary>
        public override int Length => 1;

        /// <summary>
        /// This method is used to reconstruct the filter string from its tokens.
        /// </summary>
        /// <param name="sb"></param>
        public override void Append(StringBuilder sb) => sb.Append(_comp);

        /// <summary>
        /// This is not needed.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>Always false/</returns>
        protected override bool Supports(char c) => false;
        /// <summary>
        /// This does nothing.
        /// </summary>
        protected override void WriteInternal(char c) { }

        /// <summary>
        /// This is the shortcut value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _comp.ToString();
        }
    } 
    #endregion
    #region ODataTokenStringBase
    /// <summary>
    /// This is the string based token.
    /// </summary>
    public abstract class ODataTokenStringBase : ODataTokenBase
    {
        /// <summary>
        /// This is the current data holder.
        /// </summary>
        protected StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="c">The incoming character.</param>
        protected ODataTokenStringBase(char c) : base(c)
        {
        }

        /// <summary>
        /// The length of the token.
        /// </summary>
        public override int Length => _sb.Length;

        /// <summary>
        /// This method writes the character to the internal string builder.
        /// </summary>
        /// <param name="c"></param>
        protected override void WriteInternal(char c)
        {
            _sb.Append(c);
        }

        /// <summary>
        /// This method appends the internal string array to the output.
        /// </summary>
        /// <param name="sb">The incoming string builder.</param>
        public override void Append(StringBuilder sb)
        {
            sb.Append(_sb.ToString());
        }

        /// <summary>
        /// This is the shortcut value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _sb.ToString();
        }
    }
    #endregion
    #region ODataTokenBase
    /// <summary>
    /// This is the base token class.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public abstract class ODataTokenBase
    {
        /// <summary>
        /// This constructor is for the hard coded tokens that do not take a variable.
        /// </summary>
        protected ODataTokenBase()
        {
            //Do nothing
        }
        /// <summary>
        /// This is the constructor with the initial character.
        /// </summary>
        /// <param name="c"></param>
        protected ODataTokenBase(char c)
        {
            if (!Write(c))
                throw new ArgumentOutOfRangeException($"The character '{c}' cannot be written.");
        }

        /// <summary>
        /// Specifies whether the character can be accepted at the current point in to the token.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>Returns true if accepted.</returns>
        protected abstract bool Supports(char c);

        /// <summary>
        /// Specifies whether the character can be accepted.
        /// </summary>
        /// <param name="c">The current character.</param>
        /// <returns>Returns true if the characted is accepted.</returns>
        public virtual bool Write(char c)
        {
            if (Supports(c))
            {
                WriteInternal(c);
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method is used to write the character to the buffer.
        /// </summary>
        /// <param name="c">The current character.</param>
        protected abstract void WriteInternal(char c);

        /// <summary>
        /// This is the length of the token.
        /// </summary>
        public virtual int Length => 0;

        /// <summary>
        /// This method is used to reconstruct the filter string from its tokens.
        /// </summary>
        /// <param name="sb"></param>
        public abstract void Append(StringBuilder sb);
    } 
    #endregion

}
