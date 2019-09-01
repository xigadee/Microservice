using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Xigadee
{
    /// <summary>
    /// This class builds up the necessary expression tree.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class ODataExpressionTree
    {
        #region Constructor
        /// <summary>
        /// This is the OData expression tree.
        /// </summary>
        /// <param name="tokensWithoutSeperators">The list of tokens without seperators.</param>
        public ODataExpressionTree(List<ODataTokenBase> tokensWithoutSeperators)
        {
            OriginalTokens = tokensWithoutSeperators;

            Root = new ODataExpressionNodeGroup();

            OriginalTokens.ForEach(c => Root.Write(c));
        }
        #endregion

        #region OriginalTokens
        /// <summary>
        /// This is the list of token.
        /// </summary>
        public List<ODataTokenBase> OriginalTokens { get; } 
        #endregion
        #region Root
        /// <summary>
        /// This is the root node for the expression tree.
        /// </summary>
        public ODataExpressionNodeGroup Root { get; private set; } 
        #endregion

        #region Solutions
        /// <summary>
        /// This is a list of valid solutions for the logical collection.
        /// </summary>
        public List<int> Solutions => CompileSolutions().ToList();
        #endregion

        #region CompileSolutions()
        /// <summary>
        /// This method will compile the integer solutions for the searches.
        /// </summary>
        /// <returns>Returns a list of valid integers for the bit positions of each of the searches.</returns>
        protected IEnumerable<int> CompileSolutions()
        {
            var max = Math.Pow(2, Root.HolderParams.Count);

            for (int i = 0; i < max; i++)
                if (CalculateSolution(i))
                    yield return i;
        }
        #endregion

        #region Params
        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params => Root.HolderParams.ToDictionary((h) => h.Key, (h) => h.Value.FilterParameter);
        #endregion

        protected bool CalculateSolution(int i)
        {
            return false;
        }
    }
}
