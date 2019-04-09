using System.Collections;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the collection of module that require processing.
    /// </summary>
    public class RepositoryDirectiveCollection : IEnumerable<RepositoryDirective>
    {
        /// <summary>
        /// This is the list of modules that require processing.
        /// </summary>
        public List<RepositoryDirectiveModule> Modules { get; } = new List<RepositoryDirectiveModule>();

        /// <summary>
        /// Gets a list of directives to process.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RepositoryDirective> GetEnumerator()
        {
            foreach (var module in Modules)
                foreach (var directive in module.Directives)
                    yield return directive;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
