using System;
using System.Linq.Expressions;

namespace Xigadee
{
    /// <summary>
    /// This class holds the filter expression.
    /// </summary>
    public class ODataExpressionNodeFilterParameter : ODataExpressionNodeBuild
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="components">The expression tree components.</param>
        public ODataExpressionNodeFilterParameter(ODataExpressionTree.ComponentHolder components) : base(components){}

        /// <summary>
        /// The potential negation parameter 0.
        /// </summary>
        public ODataTokenString TokenNegation => GetToken<ODataTokenString>(0);
        /// <summary>
        /// The tri-state negation option. If null then this is not the case. 
        /// </summary>
        public bool? IsNegation => (TokenNegation == null) ? default(bool?) : TokenNegation.IsKeywordNot;

        /// <summary>
        /// This is the expression parameter
        /// </summary>
        public ODataTokenString TokenParameter => GetToken<ODataTokenString>(IndexAdjust(0));
        /// <summary>
        /// This is the expression, eq, ne, gt etc.
        /// </summary>
        public ODataTokenString TokenExpression => GetToken<ODataTokenString>(IndexAdjust(1));
        /// <summary>
        /// This is the value to check against.
        /// </summary>
        public ODataTokenStringBase TokenValue => GetToken<ODataTokenStringBase>(IndexAdjust(2));

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

            if (TokenParameter == null)
                throw new ArgumentNullException("'token' cast is invalid");

            if (TokenExpression == null)
                throw new ArgumentNullException("expression cast is invalid");

            if (!TokenExpression.IsCommandExpression)
                throw new ArgumentOutOfRangeException($"expression is invalid '{TokenExpression.ToString()}'");

            if (TokenValue == null)
                throw new ArgumentNullException("'value' cast is invalid");

            var param = new FilterParameter();

            param.IsNegation = IsNegation ?? false;
            param.Parameter = TokenParameter.ToString();
            param.Operator = TokenExpression.ToString();
            param.Value = TokenValue.ToString();

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
        public override string Display => $"[{Priority}:{((IsNegation ?? false)?TokenNegation.ToString():" ")}{TokenParameter?.ToString()}|{TokenExpression?.ToString()}|{TokenValue?.ToString()}]";


        /// <summary>
        /// This expression is set to true if the bit position is matched as true.
        /// It's used to build the solution space for the query.
        /// </summary>
        /// <returns>Returns the expression.</returns>
        public override Expression<Func<int, bool>> ExpressionBuild()
        {
            var parameter = Expression.Parameter(typeof(int), "x");
            var bitPosition = Expression.Constant(1 << FilterParameter.Position, typeof(int));

            var andEx1 = Expression.And(parameter, bitPosition);
            Expression eqEx1 = Expression.Equal(andEx1, parameter);

            if (IsNegation ?? false)
                eqEx1 = Expression.Not(eqEx1);

            var finalExpression = Expression.Lambda<Func<int, bool>>(eqEx1, parameter);

            return finalExpression;
        }

    }
}
