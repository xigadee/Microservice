using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the entities and their references in to an atomic collection.
    /// This class is not optimised for high volume parallel throughput. It should only be used for testing purposes.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceEntityContainer<K,E>
        where K: IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This container holds the entities.
        /// </summary>
        Dictionary<K, E> mContainer;
        /// <summary>
        /// This container holds the key references.
        /// </summary>
        Dictionary<Tuple<string, string>, K> mContainerReference;
        /// <summary>
        /// This lock is used when modifying references.
        /// </summary>
        ReaderWriterLockSlim mReferenceModifyLock;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="referenceMaker">The reference maker.</param>
        public PersistenceEntityContainer()
        {
            mContainer = new Dictionary<K, E>();

            mContainerReference = new Dictionary<Tuple<string, string>, K>();

            mReferenceModifyLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        } 
        #endregion

        #region Count
        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return Atomic(() => mContainer.Count);
            }
        }
        #endregion
        #region CountReference
        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public int CountReference
        {
            get
            {
                return Atomic(() => mContainerReference.Count);
            }
        }
        #endregion
        #region Atomic Wrappers...
        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="action">The action to process.</param>
        private void Atomic(Action action)
        {
            try
            {
                mReferenceModifyLock.EnterReadLock();

                action();
            }
            finally
            {
                mReferenceModifyLock.ExitReadLock();
            }
        }

        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="action">The action to process.</param>
        /// <returns>Returns the value.</returns>
        private T Atomic<T>(Func<T> action)
        {
            try
            {
                mReferenceModifyLock.EnterReadLock();

                return action();
            }
            finally
            {
                mReferenceModifyLock.ExitReadLock();
            }
        } 
        #endregion

        public bool ContainsKey(K key)
        {
            return Atomic(() => mContainer.ContainsKey(key));
        }

        public bool ContainsReference(Tuple<string, string> reference)
        {
            return Atomic(() => mContainerReference.ContainsKey(reference));
        }

        /// <summary>
        /// This is the entity referemce.
        /// </summary>
        /// <param name="key">The enity key</param>
        /// <param name="value">The entity</param>
        /// <returns>
        /// 201 - Created
        /// 409 - Conflict
        /// </returns>
        public int Add(K key, E value, IEnumerable<Tuple<string, string>> references = null)
        {
            return Atomic(() =>
            {
                if (key.Equals(default(K)))
                    throw new ArgumentOutOfRangeException("key must be set to a value");
                if (value.Equals(default(E)))
                    throw new ArgumentNullException("value must be set to a value");

                //Does the key already exist in the collection
                if (mContainer.ContainsKey(key))
                    return 409;

                //Are there any references?
                if (ReferenceExistingMatch(references))
                    return 409;

                //OK, add the entity
                mContainer.Add(key, value);
                //Add the entity references
                references?.ForEach((r) => mContainerReference.Add(r, key));

                return 201;
            });
        }

        /// <summary>
        /// This method checks for existing references, but also skips matches if the associated key is set to the value passed. 
        /// This allows the method to be used for both create and update requests.
        /// </summary>
        /// <param name="references">The references to check for matches.</param>
        /// <param name="skipOnKeyMatch">A boolean property that specifies a match should be skipped if it matches the key passed in the key parameter.</param>
        /// <param name="key">The key value to skip on a match.</param>
        /// <returns>Returns true if a references has been matched to an item in the collection.</returns>
        private bool ReferenceExistingMatch(IEnumerable<Tuple<string,string>> references, bool skipOnKeyMatch = false, K key = default(K))
        {
            if (references != null && references.Count() > 0)
            {
                //And do any of the references already exist in the collection?
                foreach (var eRef in references)
                {
                    //OK, do we have a match?
                    K refKey;
                    if (mContainerReference.TryGetValue(eRef, out refKey))
                        if (!skipOnKeyMatch || !refKey.Equals(key))
                            return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method updates an existing entity.
        /// </summary>
        /// <param name="key">THe entity key.</param>
        /// <param name="newEntity">The newEntity.</param>
        /// <returns>
        /// 200 - Updated
        /// 404 - Not sound.
        /// 409 - Conflict
        /// </returns>
        public int Update(K key, E newEntity, IEnumerable<Tuple<string, string>> newReferences = null)
        {
            return Atomic(() =>
            {
                //Does the key already exist in the collection
                E oldEntity;
                if (!mContainer.TryGetValue(key, out oldEntity))
                    return 404;

                //OK, get the new references, but check whether they are assigned to another entity and if so flag an error.
                if (ReferenceExistingMatch(newReferences, true, key))
                    return 409;

                //OK, update the entity
                mContainer[key]= newEntity;
                //Remove the old references, and add the new references.
                //Note we're being lazy we add/replace even if nothing has changed.
                var oldReferences = mContainerReference.Where((r) => r.Value.Equals(key)).Select((r) => r.Key).ToList();
                oldReferences.ForEach((r) => mContainerReference.Remove(r));
                newReferences?.ForEach((r) => mContainerReference.Add(r, key));

                //All good.
                return 200;
            });
        }

        public bool Remove(K key)
        {
            return Atomic(() =>
            {
                return RemoveInternal(key);
            });
        }

        private bool RemoveInternal(K key)
        {
            bool result = mContainer.Remove(key);
            if (result)
            {
                var refs = mContainerReference.Where((r) => r.Value.Equals(key)).ToList();
                refs.ForEach((r) => mContainerReference.Remove(r.Key));
            }
            return result;
        }

        public bool Remove(Tuple<string, string> reference)
        {
            return Atomic(() =>
            {
                K key;
                if (!mContainerReference.TryGetValue(reference, out key))
                    return false;

                return RemoveInternal(key);
            });
        }

        public bool TryGetValue(K key, out E value)
        {
            value = default(E);

            E newValue = default(E);

            bool result = Atomic(() =>
            {
                return mContainer.TryGetValue(key, out newValue);
            });

            if (result)
                value = newValue;

            return result;
        }

        public bool TryGetValue(Tuple<string, string> reference, out E value)
        {
            value = default(E);

            E newValue = default(E);

            bool result = Atomic(() =>
            {
                K key;
                if (!mContainerReference.TryGetValue(reference, out key))
                    return false;

                return mContainer.TryGetValue(key, out newValue);
            });

            if (result)
                value = newValue;

            return result;
        }

        public void Clear()
        {
            Atomic(()=>
            {
                mContainer?.Clear();
                mContainerReference?.Clear();
            });
        }


        public ICollection<K> Keys
        {
            get
            {
                return Atomic(() => mContainer.Keys.ToList());
            }
        }

        public ICollection<E> Values
        {
            get
            {
                return Atomic(() => mContainer.Values.ToList());
            }
        }

        public ICollection<Tuple<string,string>> References
        {
            get
            {
                return Atomic(() => mContainerReference.Keys.ToList());
            }
        }


    }
}
