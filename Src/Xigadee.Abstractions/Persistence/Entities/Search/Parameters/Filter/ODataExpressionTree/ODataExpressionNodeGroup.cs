using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This node is used to group terms. The root node is always a group node.
    /// </summary>
    public class ODataExpressionNodeGroup : ODataExpressionNodeBuild
    {
        #region Declarations
        private bool _completed = false;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="components">The expression tree components.</param>
        public ODataExpressionNodeGroup(ODataExpressionTree.ComponentHolder components) : base(components)
        {
            Id = components.GroupRegister(this);
        }
        #endregion

        /// <summary>
        /// This the current node.
        /// </summary>
        ODataExpressionNode Current { get; set; }
        /// <summary>
        /// This is the first node for the group. 
        /// This is either the first node in the collection, or the first node after an open bracket appears.
        /// </summary>
        ODataExpressionNode First { get; set; }

        #region Write...
        /// <summary>
        /// This writes a character to the tokenizer.
        /// </summary>
        /// <param name="next">The character to write.</param>
        public override void Write(ODataTokenBase next)
        {
            if (!(Current is ODataExpressionNodeGroup))
            {
                if (next is ODataTokenBracketOpen)
                {

                    //OK, we need to swap out the current expected node in to a new Node Group.
                    var groupNext = new ODataExpressionNodeGroup(Components);
                    if (Current != null)
                    {
                        var lastNode = Current.Previous;
                        lastNode.Next = groupNext;
                        Current = groupNext;
                    }
                    return;
                }

                if (next is ODataTokenBracketClose)
                {
                    //CurrentPriority--;
                    //if (CurrentPriority < 0)
                    //    throw new ArgumentOutOfRangeException("Close bracket cannot appear before an open bracket.");

                    _completed = true;
                    return;
                }

                if (First == null)
                    NodeSetFirst();
            }

            Current.Write(next);

            if (Current.Completed)
                NodeSetNext();
        }
        #endregion

        #region NodeSetFirst()
        private void NodeSetFirst()
        {
            First = new ODataExpressionNodeFilterParameter(Components);
            Current = First;
        }
        #endregion
        #region NodeSetNext...
        private void NodeSetNext()
        {
            //Tell the node to process and internal operations.
            Current.Compile();

            if (Current is ODataExpressionNodeFilterParameter)
            {
                Components.SetFilterParameterHolder(Current);
                NodeSetNext(new ODataExpressionNodeFilterLogical(Components));
            }
            else if (Current is ODataExpressionNodeFilterLogical)
            {
                Components.SetFilterLogicalHolder(Current);
                NodeSetNext(new ODataExpressionNodeFilterParameter(Components));
            }
            else if (Current is ODataExpressionNodeGroup)
            {
                NodeSetNext(new ODataExpressionNodeFilterLogical(Components));
            }
            else
                throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Sets the next node and joins the chain.
        /// </summary>
        /// <param name="nextNode"></param>
        protected void NodeSetNext(ODataExpressionNode nextNode)
        {
            Current.Next = nextNode;
            nextNode.Previous = Current;
            Current = nextNode;
        } 
        #endregion

        /// <summary>
        /// This is the group counter.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Specifies whether the group has completed.
        /// </summary>
        public override bool Completed => _completed;

        #region Compile()
        /// <summary>
        /// This method is used to clean up the expression chain and to remove the last unused item.
        /// </summary>
        public override void Compile()
        {
            if (Current.Completed)
                throw new ArgumentOutOfRangeException("Overflow expression should not be completed on a close bracket.");

            //We're done. Remove the last unused node in the chain.
            var lastNode = Current.Previous;
            lastNode.Next = null;
            Current = lastNode;
        } 
        #endregion

        /// <summary>
        /// This is the debug display.
        /// </summary>
        public override string Display => $"Group:{Id}";



        private bool Extract(ODataExpressionNode Current, 
            out ODataExpressionNodeBuild first, out ODataExpressionNodeFilterLogical logical, out ODataExpressionNodeBuild second)
        {
            first = Current as ODataExpressionNodeBuild;
            logical = Current.Next as ODataExpressionNodeFilterLogical;
            second = Current.Next?.Next as ODataExpressionNodeBuild;

            return logical != null;
        }
        /// <summary>
        /// This method builds the expression from it's constituent parts.
        /// </summary>
        /// <returns>Returns the expression that can be called.</returns>
        public override Expression<Func<int, bool>> ExpressionBuild()
        {
            var parameter = Expression.Parameter(typeof(int), "x");

            ODataExpressionNode Current = First;

            ODataExpressionNodeBuild first;
            ODataExpressionNodeFilterLogical logical;
            ODataExpressionNodeBuild second;

            Expression currentExp = null;

            while (Extract(Current, out first, out logical, out second))
            {
                Expression Expr1 = Expression.Invoke(first.ExpressionBuild(), parameter);

                Expression Expr2 = Expression.Invoke(second.ExpressionBuild(), parameter);

                var op = logical.Expression.ToString().ToLowerInvariant();

                switch (op)
                {
                    case ODataTokenString.ODataConditionalAnd:
                        currentExp = Expression.And(currentExp ?? Expr1, Expr2);
                        break;
                    case ODataTokenString.ODataConditionalOr:
                        currentExp = Expression.Or(currentExp ?? Expr1, Expr2);
                        break;
                    case ODataTokenString.ODataConditionalXOr:
                        currentExp = Expression.ExclusiveOr(currentExp ?? Expr1, Expr2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(ExpressionBuild)}{op} is not supported.");
                }

                Current = second;
            }

            var finalExpression = Expression.Lambda<Func<int, bool>>(currentExp, parameter); 

            return finalExpression;
        }


        //Expression<Func<int, bool>> expr1 = num => (num & 1) == 1;
        //var body1 = Expression.Invoke(expr1, parameter); //x => x.Id >= 3

        //Expression<Func<int, bool>> expr2 = num => (num & 2) == 2;
        //var body2 = Expression.Invoke(expr2, parameter); //x => x.Id >= 3

        //Expression<Func<int, bool>> expr4 = num => (num & 4) == 4;
        //var body4 = Expression.Invoke(expr4, parameter); //x => x.Id >= 3

        //var andEx1 = Expression.AndAlso(body2, body1); //x.Id >= 3
        //var andEx = Expression.OrElse(andEx1, body4); //x.Id >= 3

        //var member = Expression.Property(parameter, "Id"); //x.Id
        //var constant = Expression.Constant(3);
        //var body = Expression.GreaterThanOrEqual(parameter, constant); //x.Id >= 3
        //var finalExpression = Expression.Lambda<Func<int, bool>>(body, parameter); //x => x.Id >= 3
        //var paramX = Expression.Parameter(typeof(int), "p");

        //Expression<Func<int, bool>> expr1 = num => (num & 1) == 1;

        //Expression<Func<int, bool>> expr2 = num => (num & 2) == 2;

        //BinaryExpression body = Expression.AndAlso(expr1, expr2);

        //var ExpressionTree1 = Expression.Lambda<Func<int, bool>>(body, new[] { paramX });

    }
}
