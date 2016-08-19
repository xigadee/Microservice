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
        /// <summary>
        /// This is the reference maker.
        /// </summary>
        Func<E, IEnumerable<Tuple<string, string>>> mReferenceMaker;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceMaker"></param>
        public PersistenceEntityContainer(Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null)
        {
            mReferenceMaker = referenceMaker ?? ((e) => (IEnumerable<Tuple<string, string>>)null);

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

        #region Atomic...
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
        public int Add(K key, E value)
        {
            return Atomic(() =>
            {
                //Does the key already exist in the collection
                if (mContainer.ContainsKey(key))
                    return 409;

                //Are there any references?
                var references = mReferenceMaker(value);
                if (references != null && references.Count() > 0)
                {
                    //And do any of the references already exist in the collection?
                    foreach(var eRef in references)
                        if (mContainerReference.ContainsKey(eRef))
                            return 409;
                }

                //OK, add the entity
                mContainer.Add(key, value);
                //Add the entity references
                references?.ForEach((r) => mContainerReference.Add(r, key));

                return 201;
            });
        }

        public int Update(K key, E value)
        {
            return Atomic(() =>
            {
                //Does the key already exist in the collection
                if (!mContainer.ContainsKey(key))
                    return 404;

                E oldEntity = mContainer[key];
                var oldRreferences = mReferenceMaker(oldEntity);
                var newReferences = mReferenceMaker(value);

                //Are there any references?
                var references = mReferenceMaker(value);
                if (references != null && references.Count() > 0)
                {
                    //And do any of the references already exist in the collection?
                    foreach (var eRef in references)
                        if (mContainerReference.ContainsKey(eRef))
                            return 409;
                }

                //OK, add the entity
                mContainer.Add(key, value);
                //Add the entity references
                references?.ForEach((r) => mContainerReference.Add(r, key));

                return 200;
            });
        }

        public bool Remove(K key)
        {
            return Atomic(() =>
            {
                var result = mContainer.Remove(key);
                if (result)
                {
                    var refs = mContainerReference.Where((r) => r.Value.Equals(key)).ToList();
                    refs.ForEach((r) => mContainerReference.Remove(r.Key));
                }
                return result;
            });
        }

        public bool Remove(Tuple<string, string> reference)
        {
            return Atomic(() =>
            {
                K key;
                if (!mContainerReference.TryGetValue(reference, out key))
                    return false;

                bool result = mContainer.Remove(key);
                if (result)
                {
                    var refs = mContainerReference.Where((r) => r.Value.Equals(key)).ToList();
                    refs.ForEach((r) => mContainerReference.Remove(r.Key));
                }
                return result;
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
                if (mContainerReference.TryGetValue(reference, out key))
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
