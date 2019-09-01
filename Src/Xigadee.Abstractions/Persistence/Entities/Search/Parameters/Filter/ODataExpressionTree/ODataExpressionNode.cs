using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the base expression node.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public abstract class ODataExpressionNode
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="priority">The current priority.</param>
        public ODataExpressionNode(int priority)
        {
            Priority = priority;
        }
        /// <summary>
        /// This is the priority level for the node.
        /// </summary>
        public int Priority { get; }

        public List<ODataTokenBase> Tokens { get; } = new List<ODataTokenBase>();

        public ODataExpressionNode Previous { get; set; }

        public ODataExpressionNode Next { get; set; }

        public virtual void Write(ODataTokenBase next) => Tokens.Add(next);

        public abstract bool Completed { get; }

        public abstract void Compile();

        /// <summary>
        /// This method gets a particular token at the specified position and casts it to the correct type.
        /// </summary>
        /// <typeparam name="T">The type to cast.</typeparam>
        /// <param name="pos">The collection position.</param>
        /// <returns>Returns the token of a specified type, or null if the cast was unsuccessful.</returns>
        protected T GetToken<T>(int pos) where T : ODataTokenBase => Tokens.Count > pos ? (Tokens[pos] as T) : default(T);

        /// <summary>
        /// This is the debug string for the 
        /// </summary>
        public virtual string Debug => $"{Previous?.Display ?? "NOTSET"}<-{Display}->{Next?.Display ?? "NOTSET"}";

        public abstract string Display { get; }


        Expression<Func<int, bool>> CompileNode = num => (num & 1) == 1;

    }

}
