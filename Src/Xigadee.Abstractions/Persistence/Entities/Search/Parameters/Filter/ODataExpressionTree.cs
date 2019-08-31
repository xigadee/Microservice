using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class builds up the necessary expression tree.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class ODataExpressionTree
    {
        #region Declarations
        private int CurrentPriority = 0;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the OData expression tree.
        /// </summary>
        /// <param name="tokensWithoutSeperators">The list of tokens without seperators.</param>
        public ODataExpressionTree(List<ODataTokenBase> tokensWithoutSeperators)
        {
            OriginalTokens = tokensWithoutSeperators;

            OriginalTokens.ForEach(c => Write(c));
        } 
        #endregion

        #region Root
        /// <summary>
        /// This is the root node for the expression tree.
        /// </summary>
        public ExpressionNode Root { get; private set; } 
        #endregion

        #region Current
        /// <summary>
        /// This is the current node.
        /// </summary>
        protected ExpressionNode Current { get; private set; } 
        #endregion

        #region OriginalTokens
        /// <summary>
        /// This is the list of token.
        /// </summary>
        public List<ODataTokenBase> OriginalTokens { get; } 
        #endregion

        #region Write(ODataTokenBase next)
        /// <summary>
        /// This writes a character to the tokenizer.
        /// </summary>
        /// <param name="next">The character to write.</param>
        public void Write(ODataTokenBase next)
        {
            if (next is ODataTokenBracketOpen)
            {
                CurrentPriority++;
                return;
            }

            if (next is ODataTokenBracketClose)
            {
                CurrentPriority--;
                if (CurrentPriority < 0)
                    throw new ArgumentOutOfRangeException("Close bracket cannot appear before an open bracket.");

                return;
            }

            if (Root == null)
            {
                Root = new FilterParameterHolder(CurrentPriority);
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
                    nextNode = new FilterLogicalHolder(CurrentPriority);
                }
                else if (Current is FilterLogicalHolder)
                {
                    SetFilterLogicalHolder(Current);
                    nextNode = new FilterParameterHolder(CurrentPriority);
                }
                else
                    throw new ArgumentOutOfRangeException();

                Current.Next = nextNode;
                nextNode.Previous = Current;
                Current = nextNode;
            }
        }
        #endregion

        #region SetFilterParameterHolder...
        private Dictionary<int, FilterParameterHolder> HolderParams { get; } = new Dictionary<int, FilterParameterHolder>();
        private void SetFilterParameterHolder(ExpressionNode node)
        {
            var holder = node as FilterParameterHolder;

            var param = holder.FilterParameter;
            param.Position = Params.Count + 1;
            HolderParams.Add(param.Position, holder);
        } 
        #endregion

        #region SetFilterLogicalHolder...
        private Dictionary<int, FilterLogicalHolder> HolderLogical { get; } = new Dictionary<int, FilterLogicalHolder>();
        private void SetFilterLogicalHolder(ExpressionNode node)
        {
            var holder = node as FilterLogicalHolder;

            HolderLogical.Add(HolderLogical.Count, holder);
        }
        #endregion

        #region Params
        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params => HolderParams.ToDictionary((h) => h.Key, (h) => h.Value.FilterParameter);
        #endregion

        #region Solutions
        /// <summary>
        /// This is a list of valid solutions for the logical collection.
        /// </summary>
        public List<int> Solutions => CompileSolutions().ToList(); 
        #endregion

        protected IEnumerable<int> CompileSolutions()
        {
            var max = Math.Pow(2, HolderParams.Count);

            for(int i = 0; i< max; i++)
                if (CalculateSolution(i))
                    yield return i;
        }

        protected bool CalculateSolution(int i)
        {
            return false;
        }
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

    #region FilterLogicalHolder
    /// <summary>
    /// This class holds the logical operation.
    /// </summary>
    public class FilterLogicalHolder : ExpressionNode
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="priority">The current priority.</param>
        public FilterLogicalHolder(int priority) : base(priority)
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
    #endregion

    /// <summary>
    /// This class holds the filter expression.
    /// </summary>
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
