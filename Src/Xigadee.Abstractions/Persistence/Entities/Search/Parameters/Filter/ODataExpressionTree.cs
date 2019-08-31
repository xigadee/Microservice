using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <param name="tokensWithoutSeperators">The list of tokens without seperators.</param>
        public ODataExpressionTree(List<ODataTokenBase> tokensWithoutSeperators)
        {
            OriginalTokens = tokensWithoutSeperators;

            OriginalTokens.ForEach(c => Write(c));
        }

        public ExpressionNode Root { get; private set; }

        public ExpressionNode Current { get; private set; }

        /// <summary>
        /// This is the list of token.
        /// </summary>
        public List<ODataTokenBase> OriginalTokens { get; }

        private int Priority = 0;

        /// <summary>
        /// This writes a character to the tokenizer.
        /// </summary>
        /// <param name="next">The character to write.</param>
        public void Write(ODataTokenBase next)
        {
            if (next is ODataTokenBracketOpen)
            {
                Priority++;
                return;
            }

            if (next is ODataTokenBracketClose)
            {
                Priority--;
                if (Priority < 0)
                    throw new ArgumentOutOfRangeException("Close bracket cannot appear before an open bracket.");

                return;
            }

            if (Root == null)
            {
                Root = new FilterParameterHolder(Priority);
                Current = Root;
            }

            Current.Write(next);

            if (Current.Completed)
            {
                Current.Compile();

                ExpressionNode nextNode;

                if (Current is FilterParameterHolder)
                {
                    SetFilterParameterHolder(Current);
                    nextNode = new FilterLogicalHolder(Priority);
                }
                else if (Current is FilterLogicalHolder)
                    nextNode = new FilterParameterHolder(Priority);
                else
                    throw new ArgumentOutOfRangeException();

                Current.Next = nextNode;
                nextNode.Previous = Current;
                Current = nextNode;
            }
        }

        private void SetFilterParameterHolder(ExpressionNode node)
        {
            var holder = node as FilterParameterHolder;

            var param = holder.FilterParameter;
            param.Position = Params.Count + 1;
            Params.Add(param.Position, param);
        }

        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params { get; } = new Dictionary<int, FilterParameter>();

        /// <summary>
        /// This is a list of valid solutions for the logical collection.
        /// </summary>
        public List<int> Solutions { get; } = new List<int>();
    }


    /// <summary>
    /// This is the base expression node.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public abstract class ExpressionNode
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="priority">The current priority.</param>
        public ExpressionNode(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; }

        public List<ODataTokenBase> Tokens { get; } = new List<ODataTokenBase>();

        public ExpressionNode Previous { get; set; }

        public ExpressionNode Next { get; set; }

        public virtual void Write(ODataTokenBase next) => Tokens.Add(next);

        public abstract bool Completed { get; }

        public abstract void Compile();

        protected T GetToken<T>(int pos) where T : ODataTokenBase => Tokens.Count > pos ? (Tokens[pos] as T) : default(T);

        /// <summary>
        /// This is the debug string for the 
        /// </summary>
        public virtual string Debug => $"{Previous?.Display ?? "NOTSET"}<-{Display}->{Next?.Display ?? "NOTSET"}";

        public abstract string Display {get;}
    }


    public class FilterLogicalHolder : ExpressionNode
    {
        public FilterLogicalHolder(int priority) : base(priority)
        {
        }

        /// <summary>
        /// This is either 3 tokens or 4 if the first token is a NOT token.
        /// </summary>
        public override bool Completed => Tokens.Count == 1;

        public ODataTokenString Expression => GetToken<ODataTokenString>(0);


        public override void Compile()
        {
            if (!Expression.IsCommandLogical)
                throw new ArgumentOutOfRangeException($"{Expression.ToString()} is not valid.");
        }

        public override string Display => $"[{Priority}: {Expression.ToString()}]";

    }

    public class FilterParameterHolder: ExpressionNode
    {
        public FilterParameterHolder(int priority):base(priority)
        {
        }

        public ODataTokenString Negation => GetToken<ODataTokenString>(0);

        public bool? IsNegation => (Negation == null) ? default(bool?) : Negation.IsKeywordNot;

        public ODataTokenString Parameter => GetToken<ODataTokenString>(IndexAdjust(0));

        public ODataTokenString Expression => GetToken<ODataTokenString>(IndexAdjust(1));

        public ODataTokenStringBase Value => GetToken<ODataTokenStringBase>(IndexAdjust(2));

        private T GetToken<T>(int pos) where T: ODataTokenBase => Tokens.Count > pos ? (Tokens[pos] as T) : default(T);

        private int IndexAdjust(int pos) => (IsNegation??false) ? pos + 1 : pos;

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

        public override string Display => $"[{Priority}: {FilterParameter.Display}]";

    }
}
