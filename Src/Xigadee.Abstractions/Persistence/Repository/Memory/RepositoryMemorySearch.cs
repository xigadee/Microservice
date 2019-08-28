using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This helper class is used to filter search results based on search queries.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class RepositoryMemorySearch<K, E>
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMemorySearch{K, E}"/> class.
        /// </summary>
        /// <param name="id">The search identifier.</param>
        public RepositoryMemorySearch(string id)
        {
            Id = id;
        } 
        #endregion

        #region Id
        /// <summary>
        /// Gets the search algorithm identifier.
        /// </summary>
        public string Id { get; } 
        #endregion

        #region SearchEntity...
        /// <summary>
        /// Searches the entity collection and returns a set of entities.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="sr">The search request.</param>
        /// <param name="options">The options.</param>
        public Task<IEnumerable<EntityContainerWrapper<K,E>>> SearchEntity(RepositoryMemoryContainer<K, E> collection
            , SearchRequest sr, RepositorySettings options = null)
        {
            var res = collection.Values.Select(c => new EntityContainerWrapper<K,E>(c));

            //Filter
            if (!string.IsNullOrEmpty(sr.Filter))
                res = res.Where((e) => Filter(sr, e));

            //OrderBy
            if (!string.IsNullOrEmpty(sr.OrderBy))
                res = OrderBy(sr, res);

            //Skip
            if (sr.SkipValue.HasValue)
                res = res.Skip(sr.SkipValue.Value);

            //Top
            if (sr.TopValue.HasValue)
                res = res.Take(sr.TopValue.Value);

            return Task.FromResult(res);
        }
        #endregion

        #region Filter(SearchRequest sr, EntityContainerWrapper wr)
        /// <summary>
        /// Filters the entities based on the parameters pushed and the entity properties. $filter is ignored for the default search.
        /// Logical Operators
        /// https://www.odata.org/documentation/odata-version-2-0/uri-conventions/
        /// Eq Equal	/Suppliers?$filter=Address/City eq 'Redmond'
        /// Ne Not equal	/Suppliers?$filter=Address/City ne 'London'
        /// Gt Greater than	/Products?$filter=Price gt 20
        /// Ge Greater than or equal	/Products?$filter=Price ge 10
        /// Lt Less than	/Products?$filter=Price lt 20
        /// Le Less than or equal	/Products?$filter=Price le 100
        /// And Logical and	/Products?$filter=Price le 200 and Price gt 3.5
        /// Or Logical or	/Products?$filter=Price le 3.5 or Price gt 200
        /// Not Logical negation	/Products?$filter=not endswith(Description,'milk')
        /// </summary>
        /// <param name="sr">The search request.</param>
        /// <param name="wr">The container wrapper.</param>
        /// <returns>Returns true if the filter was passed.</returns>
        protected virtual bool Filter(SearchRequest sr, EntityContainerWrapper<K, E> wr)
        {
            bool success = false;
            //bool success = true;

            ////TODo: fix memory search
            //sr.Filters.Params.ForEach(p => success &= wr.PropertyMatch(p.Key, p.Value.));
            //sr.FilterParameters.ForEach(p => success &= wr.PropertyMatch(p.Key, p.Value));

            return success;
        }
        #endregion
        #region OrderBy(SearchRequest sr, IEnumerable<EntityContainerWrapper<K,E>> container)
        /// <summary>
        /// Sets the OrderBy filters.
        /// </summary>
        /// <param name="sr">The search request.</param>
        /// <param name="container">The container.</param>
        /// <returns>The extended LINQ query.</returns>
        protected virtual IEnumerable<EntityContainerWrapper<K, E>> OrderBy(SearchRequest sr, IEnumerable<EntityContainerWrapper<K, E>> container)
        {
            var r = sr.OrderBy().ToList();
            r.Reverse();
            foreach (var res in r)
            {
                if (res.asc)
                    container = container.OrderBy(e => e.PropertyGet(res.property));
                else
                    container = container.OrderByDescending(e => e.PropertyGet(res.property));
            }

            return container;
        } 
        #endregion

    }
}
