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

        #region SearchEntity...
        /// <summary>
        /// Searches the entity collection and returns a set of entities.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="holder">The holder.</param>
        /// <param name="options">The options.</param>
        public Task SearchEntity(RepositoryMemoryContainer<K, E> collection
            , RepositoryHolder<SearchRequest, SearchResponse<E>> holder, RepositorySettings options = null)
        {
            holder.ResponseCode = 200;

            var key = holder.Key;

            var res = collection.Values.Select(c => new EntityContainerWrapper(c));

            try
            {
                //Filter
                if (!string.IsNullOrEmpty(key.Filter) || key.Parameters.Count>0)
                    res = res.Where((e) => Filter(key, e));

                //OrderBy
                if (!string.IsNullOrEmpty(key.OrderBy))
                    res = OrderBy(key, res);

                //Skip
                if (key.SkipValue.HasValue)
                    res = res.Skip(key.SkipValue.Value);

                //Top
                if (key.TopValue.HasValue)
                    res = res.Take(key.TopValue.Value);

                //Output
                holder.Entity.Data = res.Select((c) => c.Entity).ToList();
            }
            catch (Exception ex)
            {
                holder.ResponseCode = 500;
                holder.Ex = ex;
            }


            return Task.FromResult(holder);
        }
        #endregion

        /// <summary>
        /// Filters the entities based on the parameters pushed and the entity properties. $filter is ignored for the default search.
        /// </summary>
        /// <param name="sr">The search request.</param>
        /// <param name="wr">The container wrapper.</param>
        /// <returns>Returns true if the filter was passed.</returns>
        protected virtual bool Filter(SearchRequest sr, EntityContainerWrapper wr)
        {
            bool success = true;

            sr.Parameters.ForEach(p => success &= wr.PropertyMatch(p.Key, p.Value));

            return success;
        }

        protected virtual IEnumerable<EntityContainerWrapper> OrderBy(SearchRequest sr, IEnumerable<EntityContainerWrapper> container)
        {

            return container;
        }

        #region Class -> EntityContainerWrapper
        /// <summary>
        /// This wrapper class is used to stop multiple deserializations of an entity when filtering a results set.
        /// </summary>
        protected class EntityContainerWrapper
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EntityContainerWrapper"/> class. That holds a cached deserialized version of the entity.
            /// </summary>
            /// <param name="c">The collection.</param>
            public EntityContainerWrapper(EntityContainer<K, E> c)
            {
                Container = c;
            }

            private E _entity;
            private bool _interned = false;

            /// <summary>
            /// Gets the container.
            /// </summary>
            public EntityContainer<K, E> Container { get; }

            /// <summary>
            /// Gets the cached deserialized entity.
            /// </summary>
            public E Entity
            {
                get
                {
                    if (!_interned)
                    {
                        _entity = Container.Entity;
                        _interned = true;
                    }
                    return _entity;
                }
            }

            /// <summary>
            /// Checks a property for a match.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public bool PropertyMatch(string key, string value)
            {
                try
                {
                    var result = Container.Properties
                        .FirstOrDefault(p => p.Item1.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                    return result!=null && (result.Item2?.Equals(value) ?? false);
                }
                catch (Exception)
                {

                    return false; ;
                }

            }
        } 
        #endregion
    }
}
