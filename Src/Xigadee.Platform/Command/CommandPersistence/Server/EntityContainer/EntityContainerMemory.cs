using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the entities and their references in to an atomic collection.
    /// This class is not optimised for high volume parallel throughput. It should only be used for testing purposes.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    [DebuggerDisplay("{Debug}")]
    public class EntityContainerMemory<K,E>: EntityContainerBase<K,E>
        where K: IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This container holds the entities.
        /// </summary>
        Dictionary<K, EntityContainer> mContainer;
        /// <summary>
        /// This container holds the key references.
        /// </summary>
        Dictionary<Tuple<string, string>, EntityContainer> mContainerReference;
        /// <summary>
        /// This lock is used when modifying references.
        /// </summary>
        ReaderWriterLockSlim mReferenceModifyLock;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public EntityContainerMemory()
        {
            mContainer = new Dictionary<K, EntityContainer>();

            mContainerReference = new Dictionary<Tuple<string, string>, EntityContainer>(new ReferenceComparer());

        }
        #endregion

        #region StopInternal()
        /// <summary>
        /// This method stops the service. The override also clears the memory array of all data.
        /// </summary>
        protected override void StopInternal()
        {
            Clear();
            base.StopInternal();
        }
        #endregion

        #region Count
        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public override int Count
        {
            get
            {
                return Atomic(() => mContainer.Count);
            }
        }
        #endregion
        #region CountReference
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        public override int CountReference
        {
            get
            {
                return Atomic(() => mContainerReference.Count);
            }
        }
        #endregion

        #region Class -> ReferenceComparer
        /// <summary>
        /// This helper class is used to ensure that references are matched in a case insensitive manner.
        /// </summary>
        private class ReferenceComparer: IEqualityComparer<Tuple<string, string>>
        {
            public bool Equals(Tuple<string, string> x, Tuple<string, string> y)
            {
                return string.Equals(x?.Item1, y?.Item1, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x?.Item2, y?.Item2, StringComparison.CurrentCultureIgnoreCase);
            }

            public int GetHashCode(Tuple<string, string> obj)
            {
                //We create a lower-case invariant tuple and return its hash code. This will make Equals fire for comparison.
                var invariantTuple = new Tuple<string, string>(obj?.Item1?.ToLowerInvariant(), obj?.Item2?.ToLowerInvariant());
                return invariantTuple.GetHashCode();
            }
        } 
        #endregion
        #region Class -> EntityContainer
        /// <summary>
        /// This is a private class it is used to ensure that we do not duplicate data.
        /// </summary>
        private class EntityContainer
        {
            public EntityContainer(K key, E entity, IEnumerable<Tuple<string, string>> references)
            {
                Key = key;
                Entity = entity;
                References = references == null ? new List<Tuple<string, string>>() : references.ToList();
            }

            public K Key { get; }

            public E Entity { get; }

            public List<Tuple<string, string>> References { get; }
        }
        #endregion

        #region ContainsKey(K key)
        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the collection contains the key; otherwise, <c>false</c>.
        /// </returns>
        public override bool ContainsKey(K key)
        {
            return Atomic(() => mContainer.ContainsKey(key));
        }
        #endregion
        #region ContainsReference(Tuple<string, string> reference)
        /// <summary>
        /// Determines whether the collection contains the entity reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        ///   <c>true</c> if the collection contains reference; otherwise, <c>false</c>.
        /// </returns>
        public override bool ContainsReference(Tuple<string, string> reference)
        {
            return Atomic(() => mContainerReference.ContainsKey(reference));
        }
        #endregion


        #region Add(K key, E value, IEnumerable<Tuple<string, string>> references = null)
        /// <summary>
        /// This is the entity reference.
        /// </summary>
        /// <param name="key">The entity key</param>
        /// <param name="value">The entity</param>
        /// <param name="references">The optional references.</param>
        /// <returns>
        /// 201 - Created
        /// 409 - Conflict
        /// </returns>
        public override int Add(K key, E value, IEnumerable<Tuple<string, string>> references = null)
        {
            return Atomic(() =>
            {
                IncomingParameterChecks(key, value);

                //Does the key already exist in the collection
                if (mContainer.ContainsKey(key))
                    return 409;

                //Are there any references?
                if (ReferenceExistingMatch(references))
                    return 409;

                var container = new EntityContainer(key, value, references);
                //OK, add the entity
                mContainer.Add(key, container);
                //Add the entity references
                container.References.ForEach((r) => mContainerReference.Add(r, container));

                return 201;
            });
        }
        #endregion
        #region ReferenceExistingMatch(IEnumerable<Tuple<string,string>> references, bool skipOnKeyMatch = false, K key = default(K))
        /// <summary>
        /// This method checks for existing references, but also skips matches if the associated key is set to the value passed. 
        /// This allows the method to be used for both create and update requests.
        /// </summary>
        /// <param name="references">The references to check for matches.</param>
        /// <param name="skipOnKeyMatch">A boolean property that specifies a match should be skipped if it matches the key passed in the key parameter.</param>
        /// <param name="key">The key value to skip on a match.</param>
        /// <returns>Returns true if a references has been matched to an item in the collection.</returns>
        private bool ReferenceExistingMatch(IEnumerable<Tuple<string, string>> references, bool skipOnKeyMatch = false, K key = default(K))
        {
            if (references != null && references.Count() > 0)
            {
                //And do any of the references already exist in the collection?
                foreach (var eRef in references)
                {
                    //OK, do we have a match?
                    EntityContainer refKey;
                    if (mContainerReference.TryGetValue(eRef, out refKey))
                        if (!skipOnKeyMatch || !refKey.Key.Equals(key))
                            return true;
                }
            }

            return false;
        }
        #endregion
        #region Update(K key, E newEntity, IEnumerable<Tuple<string, string>> newReferences = null)
        /// <summary>
        /// This method updates an existing entity.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="value">The new entity value.</param>
        /// <param name="newReferences">The optional new references.</param>
        /// <returns>
        /// 200 - Updated
        /// 404 - Not sound.
        /// 409 - Conflict
        /// </returns>
        public override int Update(K key, E value, IEnumerable<Tuple<string, string>> newReferences = null)
        {
            return Atomic(() =>
            {
                IncomingParameterChecks(key, value);

                //Does the key already exist in the collection
                EntityContainer oldContainer;
                if (!mContainer.TryGetValue(key, out oldContainer))
                    return 404;

                //OK, get the new references, but check whether they are assigned to another entity and if so flag an error.
                if (ReferenceExistingMatch(newReferences, true, key))
                    return 409;

                var newContainer = new EntityContainer(key, value, newReferences);

                //OK, update the entity
                mContainer[key] = newContainer;
                //Remove the old references, and add the new references.
                //Note we're being lazy we add/replace even if nothing has changed.
                oldContainer.References.ForEach((r) => mContainerReference.Remove(r));
                newContainer.References.ForEach((r) => mContainerReference.Add(r, newContainer));

                //All good.
                return 200;
            });
        }
        #endregion
        #region Remove(K key)
        /// <summary>
        /// Removes an entity with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns true if the entity is removed successfully.</returns>
        public override bool Remove(K key)
        {
            return Atomic(() =>
            {
                IncomingParameterChecks(key);
                EntityContainer container = null;
                if (mContainer.TryGetValue(key, out container))
                    return RemoveInternal(container);

                return false;
            });
        }
        #endregion
        #region Remove(Tuple<string, string> reference)
        /// <summary>
        /// Removes the specified entities by the supplied reference.
        /// </summary>
        /// <param name="reference">The reference identifier.</param>
        /// <returns>Returns true if the entity is removed successfully.</returns>
        public override bool Remove(Tuple<string, string> reference)
        {
            return Atomic(() =>
            {
                EntityContainer container;
                if (!mContainerReference.TryGetValue(reference, out container))
                    return false;

                return RemoveInternal(container);
            });
        } 
        #endregion

        private bool RemoveInternal(EntityContainer container)
        {
            bool result = mContainer.Remove(container.Key);
            if (result)
                container.References.ForEach((r) => mContainerReference.Remove(r));

            return result;
        }

        #region TryGetValue(K key, out E value)
        /// <summary>
        /// Tries to retrieve and entity by the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The output value.</param>
        /// <returns>True if the key exists.</returns>
        public override bool TryGetValue(K key, out E value)
        {
            IncomingParameterChecks(key);
            value = default(E);

            EntityContainer newValue = null; ;

            bool result = Atomic(() =>
            {
                return mContainer.TryGetValue(key, out newValue);
            });

            if (result)
                value = newValue == null ? default(E) : newValue.Entity;

            return result;
        }
        #endregion
        #region TryGetValue(Tuple<string, string> reference, out E value)
        /// <summary>
        /// Tries to retrieve and entity by the reference id.
        /// </summary>
        /// <param name="reference">The reference id.</param>
        /// <param name="value">The output value.</param>
        /// <returns>True if the reference exists.</returns>
        public override bool TryGetValue(Tuple<string, string> reference, out E value)
        {
            value = default(E);

            EntityContainer newValue = null; ;

            bool result = Atomic(() =>
            {
                return mContainerReference.TryGetValue(reference, out newValue);
            });

            if (result)
                value = newValue == null ? default(E) : newValue.Entity;

            return result;
        }
        #endregion

        #region Clear()
        /// <summary>
        /// Clears this collection of all entities and references.
        /// </summary>
        public override void Clear()
        {
            Atomic(() =>
            {
                mContainer?.Clear();
                mContainerReference?.Clear();
            });
        }
        #endregion

        #region Keys
        /// <summary>
        /// Gets the keys collection.
        /// </summary>
        public override ICollection<K> Keys
        {
            get
            {
                return Atomic(() => mContainer.Keys.ToList());
            }
        }
        #endregion
        #region Values
        /// <summary>
        /// Gets the values collection.
        /// </summary>
        public override ICollection<E> Values
        {
            get
            {
                return Atomic(() => mContainer.Values.Select((c) => c.Entity).ToList());
            }
        }
        #endregion
        #region References
        /// <summary>
        /// Gets the references collection.
        /// </summary>
        public override ICollection<Tuple<string, string>> References
        {
            get
            {
                return Atomic(() => mContainerReference.Keys.ToList());
            }
        }
        #endregion
    }
}
