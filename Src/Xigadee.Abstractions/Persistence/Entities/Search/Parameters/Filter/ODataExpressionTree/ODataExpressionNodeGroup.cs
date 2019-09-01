using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This node is used to group terms. The root node is always a group node.
    /// </summary>
    public class ODataExpressionNodeGroup : ODataExpressionNode
    {
        #region Declarations
        private int CurrentPriority = 0;
        private int GroupPriority = 0;

        private bool _completed = false;
        #endregion
        #region Constructor
        /// <summary>
        /// This is teh default constructor.
        /// </summary>
        public ODataExpressionNodeGroup() : this(0, 0)
        {

        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="id">The group id.</param>
        /// <param name="priority">The group priority.</param>
        public ODataExpressionNodeGroup(int id, int priority) : base(priority)
        {
            Id = id;
        }
        #endregion

        ODataExpressionNode Current { get; set; }

        ODataExpressionNode Root { get; set; }

        #region Write...
        /// <summary>
        /// This writes a character to the tokenizer.
        /// </summary>
        /// <param name="next">The character to write.</param>
        public override void Write(ODataTokenBase next)
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
                Root = new ODataExpressionNodeFilterParameter(CurrentPriority);
                Current = Root;
            }

            Current.Write(next);

            if (Current.Completed)
            {
                Current.Compile();

                ODataExpressionNode nextNode;

                if (Current is ODataExpressionNodeFilterParameter)
                {
                    SetFilterParameterHolder(Current);
                    nextNode = new ODataExpressionNodeFilterLogical(CurrentPriority);
                }
                else if (Current is ODataExpressionNodeFilterLogical)
                {
                    SetFilterLogicalHolder(Current);
                    nextNode = new ODataExpressionNodeFilterParameter(CurrentPriority);
                }
                else
                    throw new ArgumentOutOfRangeException();

                Current.Next = nextNode;
                nextNode.Previous = Current;
                Current = nextNode;
            }
        }
        #endregion

        /// <summary>
        /// This is the group counter.
        /// </summary>
        public int Id { get; }

        public override bool Completed => _completed;

        public override string Display => throw new NotImplementedException();

        public override void Compile()
        {
            throw new NotImplementedException();
        }


        #region SetFilterParameterHolder...
        public Dictionary<int, ODataExpressionNodeFilterParameter> HolderParams { get; } = new Dictionary<int, ODataExpressionNodeFilterParameter>();

        private void SetFilterParameterHolder(ODataExpressionNode node)
        {
            var holder = node as ODataExpressionNodeFilterParameter;

            var param = holder.FilterParameter;
            param.Position = HolderParams.Count + 1;
            HolderParams.Add(param.Position, holder);
        }
        #endregion
        #region SetFilterLogicalHolder...
        public Dictionary<int, ODataExpressionNodeFilterLogical> HolderLogical { get; } = new Dictionary<int, ODataExpressionNodeFilterLogical>();

        private void SetFilterLogicalHolder(ODataExpressionNode node)
        {
            var holder = node as ODataExpressionNodeFilterLogical;

            HolderLogical.Add(HolderLogical.Count, holder);
        }
        #endregion

        public override string Debug => $"Group {Id}";
    }
}
