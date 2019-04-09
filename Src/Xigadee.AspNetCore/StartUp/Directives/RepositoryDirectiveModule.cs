using System.Collections.Generic;

namespace Xigadee
{
    #region RepositoryDirectiveModule
    /// <summary>
    /// This class holds a specific module and the associated directives.
    /// </summary>
    public class RepositoryDirectiveModule
    {
        /// <summary>
        /// This is the root module.
        /// </summary>
        public object Module { get; set; }
        /// <summary>
        /// This is the list of repository populate directives.
        /// </summary>
        public List<RepositoryDirective> Directives { get; } = new List<RepositoryDirective>();
    }
    #endregion
}
