using System;
using System.Collections.Generic;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This class holds the memory container.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class RepositoryMemoryContainer<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        private long _etagOrdinal = 0;
        /// <summary>
        /// This container holds the entities.
        /// </summary>
        protected readonly Dictionary<K, EntityContainer<K, E>> _container;
        /// <summary>
        /// This container holds the key references.
        /// </summary>
        protected readonly Dictionary<Tuple<string, string>, EntityContainer<K, E>> _containerReference;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMemoryContainer{K, E}"/> class.
        /// </summary>
        public RepositoryMemoryContainer()
        {
            _container = new Dictionary<K, EntityContainer<K, E>>();
            _containerReference = new Dictionary<Tuple<string, string>, EntityContainer<K, E>>(new ReferenceComparer());
        }
        #endregion

        #region Add(EntityContainer<K, E> newContainer)
        /// <summary>
        /// Add an entity container to the collection.
        /// </summary>
        /// <param name="newContainer"></param>
        /// <returns></returns>
        public bool Add(EntityContainer<K, E> newContainer)
        {
            var key = newContainer.Key;
            //Does the key already exist in the collection
            if (Contains(key))
                return false;

            //Are there any references? And do they already exist.
            if (ReferenceExistingMatch(newContainer.References))
                return false;

            //OK, add the entity
            _container.Add(key, newContainer);

            //Add the entity references
            newContainer.References.ForEach((r) => _containerReference.Add(r, newContainer));

            ETagOrdinalIncrement();

            return true;
        }
        #endregion
        #region Replace(EntityContainer<K, E> oldContainer, EntityContainer<K, E> newContainer)
        /// <summary>
        /// Replace a container for an entity.
        /// </summary>
        /// <param name="oldContainer"></param>
        /// <param name="newContainer"></param>
        public void Replace(EntityContainer<K, E> oldContainer, EntityContainer<K, E> newContainer)
        {
            var key = oldContainer.Key;

            if (!key.Equals(newContainer.Key))
                throw new ArgumentOutOfRangeException($"Container keys do not match: {key}/{newContainer.Key}");

            //OK, update the entity
            _container[key] = newContainer;

            //Remove the old references, and add the new references.
            //Note we're being lazy we add/replace even if nothing has changed.
            oldContainer.References.ForEach((r) => _containerReference.Remove(r));
            newContainer.References.ForEach((r) => _containerReference.Add(r, newContainer));
        }
        #endregion

        /// <summary>
        /// Checks whether the collection contains the key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists.</returns>
        public bool Contains(K key) => _container.ContainsKey(key);
        /// <summary>
        /// Checks whether the collection contains the entity reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>True if the reference exists.</returns>
        public bool Contains(Tuple<string, string> reference) => _containerReference.ContainsKey(reference);
        /// <summary>
        /// Checks for a container.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="container">The container as an output.</param>
        /// <returns>True if the key exists.</returns>
        public bool TryGetValue(K key, out EntityContainer<K, E> container)
        {
            return _container.TryGetValue(key, out container);
        }

        /// <summary>
        /// Checks for a container.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="container">The container as an output.</param>
        /// <returns>True if the entity reference exists.</returns>
        public bool TryGetValue(Tuple<string, string> reference, out EntityContainer<K, E> container)
        {
            return _containerReference.TryGetValue(reference, out container);
        }

        /// <summary>
        /// Deletes a container.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="container">The container as an output.</param>
        /// <returns>True if the container exists and has been deleted.</returns>
        public bool Delete(K key, out EntityContainer<K, E> container)
        {
            if (_container.TryGetValue(key, out container))
                return DeleteInternal(container);

            container = null;
            return false;
        }

        /// <summary>
        /// Deletes a container.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="container">The container as an output.</param>
        /// <returns>True if the container exists and has been deleted.</returns>
        public bool Delete(Tuple<string, string> reference, out EntityContainer<K, E> container)
        {
            if (_containerReference.TryGetValue(reference, out container))
                return DeleteInternal(container);

            container = null;
            return false;
        }

        private bool DeleteInternal(EntityContainer<K, E> container)
        {
            bool result = _container.Remove(container.Key);

            if (result)
            {
                container.References.ForEach((r) => _containerReference.Remove(r));

                ETagOrdinalIncrement();
            }

            return result;
        }

        #region Count
        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public virtual int Count => _container.Count;
        #endregion
        #region CountReference
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        public virtual int CountReference => _containerReference.Count;
        #endregion
        #region ReferenceExistingMatch...
        /// <summary>
        /// This method checks for existing references, but also skips matches if the associated key is set to the value passed. 
        /// This allows the method to be used for both create and update requests.
        /// </summary>
        /// <param name="references">The references to check for matches.</param>
        /// <param name="skipOnKeyMatch">A boolean property that specifies a match should be skipped if it matches the key passed in the key parameter.</param>
        /// <param name="key">The key value to skip on a match.</param>
        /// <returns>Returns true if a references has been matched to an item in the collection.</returns>
        public bool ReferenceExistingMatch(IList<Tuple<string, string>> references, bool skipOnKeyMatch = false, K key = default(K))
        {
            if (references != null && references.Count > 0)
            {
                //And do any of the references already exist in the collection?
                foreach (var eRef in references)
                {
                    //OK, do we have a match?
                    if (_containerReference.TryGetValue(eRef, out var refKey))
                        if (!skipOnKeyMatch || !refKey.Key.Equals(key))
                            return true;
                }
            }

            return false;
        }
        #endregion

        #region ETagOrdinal
        /// <summary>
        /// Gets the current collection ETag. This changes when an entity is created/updated or deleted.
        /// </summary>
        public long ETagOrdinal => _etagOrdinal;
        #endregion
        #region ETagOrdinalIncrement()
        /// <summary>
        /// This method is called when the collection is changed.
        /// </summary>
        protected virtual void ETagOrdinalIncrement()
        {
            Interlocked.Increment(ref _etagOrdinal);
        }
        #endregion

        #region Class -> ReferenceComparer
        /// <summary>
        /// This helper class is used to ensure that references are matched in a case insensitive manner.
        /// </summary>
        protected class ReferenceComparer : IEqualityComparer<Tuple<string, string>>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type T to compare.</param>
            /// <param name="y">The second object of type T to compare.</param>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            public bool Equals(Tuple<string, string> x, Tuple<string, string> y)
            {
                return string.Equals(x?.Item1, y?.Item1, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x?.Item2, y?.Item2, StringComparison.CurrentCultureIgnoreCase);
            }
            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(Tuple<string, string> obj)
            {
                //We create a lower-case invariant tuple and return its hash code. This will make Equals fire for comparison.
                var invariantTuple = new Tuple<string, string>(obj?.Item1?.ToLowerInvariant(), obj?.Item2?.ToLowerInvariant());
                return invariantTuple.GetHashCode();
            }
        }
        #endregion

    }
}
