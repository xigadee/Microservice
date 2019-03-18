using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMemorySearch{K, E}"/> class.
        /// </summary>
        /// <param name="id">The search identifier.</param>
        public RepositoryMemorySearch(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the search algorithm identifier.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Searches the entity collection and returns a set of entities.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="holder">The holder.</param>
        /// <param name="options">The options.</param>
        public Task SearchEntity(RepositoryMemoryContainer<K, E> collection, RepositoryHolder<SearchRequest, SearchResponse<E>> holder, RepositorySettings options = null)
        {
            holder.ResponseCode = 400;

            //if (rq?.Id == null || !_filterMethods.ContainsKey(rq.Id.Trim().ToLowerInvariant())


            //Func<E, List<KeyValuePair<string, string>>, bool> filter;

            //if (string.IsNullOrEmpty(key.Query))//The default filter returns all records.
            //    filter = (e, p) => true;
            //else if (_filterMethods.ContainsKey(key.Query))
            //    filter = _filterMethods[key.Query];
            //else
            //    return Task.FromResult(new RepositoryHolder<SearchRequest, SearchResponse<E>>(key, response, 404));

            //response.Data = Atomic(() =>
            //{
            //    var res = _container.Values
            //    .Where((e) => filter(e.Entity, key.FilterParams))
            //    .Select((c) => c.Entity);

            //    if (key.Skip.HasValue)
            //        res = res.Skip(key.Top.Value);

            //    if (key.Top.HasValue)
            //        res = res.Take(key.Top.Value);

            //    return res.ToList();
            //});

            return Task.FromResult(holder);
        }

        //Func<E, List<KeyValuePair<string, string>>, bool>
    }

}
