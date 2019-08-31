using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class builds up the necessary expression tree.
    /// </summary>
    public class ODataExpressionTree
    {
        /// <summary>
        /// This is the OData expression tree.
        /// </summary>
        /// <param name="TokensWithoutSeperators">The list of tokens without seperators.</param>
        public ODataExpressionTree(List<ODataTokenBase> TokensWithoutSeperators)
        {
            OriginalTokens = TokensWithoutSeperators;

            OriginalTokens.ForEach(c => Write(c));


            //var filterParsed = BracketDefinition.Parse(filter, out var bracketPositions);
            //Params = SearchRequestHelper.BuildParameters<FilterParameter>(filterParsed, FilterLogical.ODataConditionals).ToDictionary(r => r.Position, r => r);
            //Solutions = CalculateSolutions(filter);
        }

        /// <summary>
        /// This is the list of token.
        /// </summary>
        public List<ODataTokenBase> OriginalTokens = new List<ODataTokenBase>();

        /// <summary>
        /// This is the list of token.
        /// </summary>
        public List<ODataTokenBase> Expressions = new List<ODataTokenBase>();

        /// <summary>
        /// This is the first token in the chain.
        /// </summary>
        public ODataTokenBase First => OriginalTokens.Count > 0 ? OriginalTokens[0] : null;

        /// <summary>
        /// This is the current token that can be written to.
        /// </summary>
        public ODataTokenBase Current => OriginalTokens.Count > 0 ? OriginalTokens[OriginalTokens.Count - 1] : null;

        /// <summary>
        /// This writes a character to the tokenizer.
        /// </summary>
        /// <param name="next">The character to write.</param>
        public void Write(ODataTokenBase next)
        {
            ////Keep writing until the token says it cannot accept any more.
            //if (Current != null && Current.Write(next))
            //    return;

            ////OK, we need to create a new token
            //if (ODataTokenSeperator.CanStart(next))
            //    Tokens.Add(new ODataTokenSeperator(next));
            ////Open bracket
            //else if (ODataTokenBracketOpen.CanStart(next))
            //    Tokens.Add(new ODataTokenBracketOpen(next));
            ////Close bracket
            //else if (ODataTokenBracketClose.CanStart(next))
            //    Tokens.Add(new ODataTokenBracketClose(next));
            ////Quoted string
            //else if (ODataTokenQuotedString.CanStart(next))
            //    Tokens.Add(new ODataTokenQuotedString(next));
            ////Token
            //else if (ODataTokenString.CanStart(next))
            //    Tokens.Add(new ODataTokenString(next));
            //else
            //    throw new ArgumentOutOfRangeException($"Invalid character {next} at position {Length}");
        }

        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params { get; }

        /// <summary>
        /// This is a list of valid solutions for the logical collection.
        /// </summary>
        public List<int> Solutions { get; }
    }
}
