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
    [DebuggerDisplay("{DebugDisplay}")]
    public abstract class ODataExpressionNode
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="components">The expression tree components.</param>
        public ODataExpressionNode(ODataExpressionTree.ComponentHolder components)
        {
            Components = components;
            Priority = components.NextComponentId();
        }
        /// <summary>
        /// This is the priority level for the node.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// This is the set of expression tree components.
        /// </summary>
        public ODataExpressionTree.ComponentHolder Components { get; }
        /// <summary>
        /// The list of tokens for this node.
        /// </summary>
        public List<ODataTokenBase> Tokens { get; } = new List<ODataTokenBase>();
        /// <summary>
        /// The previous node in the chain.
        /// </summary>
        public ODataExpressionNode Previous { get; set; }
        /// <summary>
        /// The next node in the chain.
        /// </summary>
        public ODataExpressionNode Next { get; set; }
        /// <summary>
        /// This method writes the next token to the node.
        /// </summary>
        /// <param name="next"></param>
        public virtual void Write(ODataTokenBase next) => Tokens.Add(next);
        /// <summary>
        /// This method switches to true when the node has reached the required number of tokens.
        /// </summary>
        public abstract bool Completed { get; }
        /// <summary>
        /// This method is called when the node is marked as completed.
        /// </summary>
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
        public virtual string Debug => $"{Previous?.DebugDisplay ?? "NOTSET"}<-{DebugDisplay}->{Next?.DebugDisplay ?? "NOTSET"}";
        /// <summary>
        /// This is the debug display.
        /// </summary>
        public abstract string DebugDisplay { get; }
    }

    /// <summary>
    /// This class is implemented by builders that return an expression based on their contents.
    /// </summary>
    public abstract class ODataExpressionNodeBuild : ODataExpressionNode
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="components">The expression tree components.</param>
        public ODataExpressionNodeBuild(ODataExpressionTree.ComponentHolder components):base(components){}

        /// <summary>
        /// This method builds the expression from it's constituent parts.
        /// </summary>
        /// <returns>Returns the expression that can be called.</returns>
        public abstract Expression<Func<int, bool>> ExpressionBuild();

        /// <summary>
        /// This method compiles the expression to a specific function.
        /// </summary>
        /// <returns>Returns the actual compiled function.</returns>
        public Func<int, bool> CompileToExpression() => ExpressionBuild().Compile();
    }

}
