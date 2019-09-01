using System;

namespace Xigadee
{
    /// <summary>
    /// This class holds the filter expression.
    /// </summary>
    public class ODataExpressionNodeFilterParameter : ODataExpressionNode
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="components">The expression tree components.</param>
        public ODataExpressionNodeFilterParameter(ODataExpressionTree.ComponentHolder components) : base(components)
        {
        }

        /// <summary>
        /// The potential negation parameter 0.
        /// </summary>
        public ODataTokenString Negation => GetToken<ODataTokenString>(0);
        /// <summary>
        /// The tri-state negation option. If null then this is not the case. 
        /// </summary>
        public bool? IsNegation => (Negation == null) ? default(bool?) : Negation.IsKeywordNot;

        /// <summary>
        /// This is the expression parameter
        /// </summary>
        public ODataTokenString Parameter => GetToken<ODataTokenString>(IndexAdjust(0));
        /// <summary>
        /// This is the expression, eq, ne, gt etc.
        /// </summary>
        public ODataTokenString Expression => GetToken<ODataTokenString>(IndexAdjust(1));
        /// <summary>
        /// This is the value to check against.
        /// </summary>
        public ODataTokenStringBase Value => GetToken<ODataTokenStringBase>(IndexAdjust(2));

        /// <summary>
        /// This method adjusts the index. This is necessary when the first token is a 'not' as this shifts up the other
        /// tokens by one place in the collection.
        /// </summary>
        /// <param name="pos">The original position.</param>
        /// <returns>The adjusted position if the first position is a not token.</returns>
        private int IndexAdjust(int pos) => (IsNegation ?? false) ? pos + 1 : pos;

        /// <summary>
        /// This method compiles the tokens in to a FilterParameter.
        /// </summary>
        public override void Compile()
        {
            if (Tokens.Count < 3 || Tokens.Count > 4)
                throw new ArgumentOutOfRangeException("Incorrect parameters.");

            if (Tokens.Count == 4)
            {
                if (!(IsNegation ?? false))
                    throw new ArgumentException("Invalid negation token.");
            }

            if (Parameter == null)
                throw new ArgumentNullException("'token' cast is invalid");

            if (Expression == null)
                throw new ArgumentNullException("expression cast is invalid");

            if (!Expression.IsCommandExpression)
                throw new ArgumentOutOfRangeException($"expression is invalid '{Expression.ToString()}'");

            if (Value == null)
                throw new ArgumentNullException("'value' cast is invalid");

            var param = new FilterParameter();

            param.IsNegation = IsNegation ?? false;
            param.Parameter = Parameter.ToString();
            param.Operator = Expression.ToString();
            param.Value = Value.ToString();

            FilterParameter = param;
        }

        /// <summary>
        /// This is the compiled filter parameter.
        /// </summary>
        public FilterParameter FilterParameter { get; private set; }

        /// <summary>
        /// This is either 3 tokens or 4 if the first token is a NOT token.
        /// </summary>
        public override bool Completed => ((IsNegation ?? false) && Tokens.Count == 4) || (!(IsNegation ?? false) && Tokens.Count == 3);
        /// <summary>
        /// This is the node debug display.
        /// </summary>
        public override string Display => $"[{Priority}:{((IsNegation ?? false)?Negation.ToString():" ")}{Parameter?.ToString()}|{Expression?.ToString()}|{Value?.ToString()}]";

    }
}
