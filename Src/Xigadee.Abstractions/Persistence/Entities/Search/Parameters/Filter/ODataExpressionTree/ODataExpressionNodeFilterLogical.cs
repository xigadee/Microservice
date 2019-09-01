using System;

namespace Xigadee
{
    /// <summary>
    /// This class holds the logical operation.
    /// </summary>
    public class ODataExpressionNodeFilterLogical : ODataExpressionNode
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="priority">The current priority.</param>
        public ODataExpressionNodeFilterLogical(int priority) : base(priority)
        {
        }

        /// <summary>
        /// This is either 3 tokens or 4 if the first token is a NOT token.
        /// </summary>
        public override bool Completed => Tokens.Count == 1;

        /// <summary>
        /// The logical expression.
        /// </summary>
        public ODataTokenString Expression => GetToken<ODataTokenString>(0);

        /// <summary>
        /// Complete the compilation of the node.
        /// </summary>
        public override void Compile()
        {
            if (!Expression.IsCommandLogical)
                throw new ArgumentOutOfRangeException($"{Expression.ToString()} is not valid.");
        }

        /// <summary>
        /// This is the debug display.
        /// </summary>
        public override string Display => $"[{Priority}: {Expression.ToString()}]";

    }
}
