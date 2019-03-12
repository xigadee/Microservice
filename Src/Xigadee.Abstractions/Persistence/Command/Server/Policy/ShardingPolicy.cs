#region using

using System;
using System.Collections.Generic;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to shard the collection based on the key passed.
    /// </summary>
    /// <typeparam name="K">The collection key type.</typeparam>
    public class ShardingPolicy<K>:PolicyBase
        where K : IEquatable<K>
    {
        #region Declarations
        private Func<K, int> mFnPartition;
        private Func<int, string> mFnPartitionNameMap;
        private Dictionary<int, string> mPartitionCache;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the sharding policy.
        /// </summary>
        /// <param name="collectionBase">The collection base string.</param>
        /// <param name="fnPartition">The function that transforms the incoming key in to the numeric partition.</param>
        /// <param name="partitionCount">This is maximum number of partitions to create.</param>
        public ShardingPolicy(string collectionBase, Func<K, int> fnPartition, int partitionCount, Func<int, string> fnPartitionNameMap = null)
        {
            if (collectionBase == null)
                throw new ArgumentNullException("collectionBase");
            if (fnPartition == null)
                throw new ArgumentNullException("fnPartition");
            if (partitionCount <= 0)
                throw new ArgumentOutOfRangeException("partitionCount must be greater than 0.");

            CollectionBase = collectionBase;
            PartitionCount = partitionCount;

            mFnPartition = fnPartition;
            mFnPartitionNameMap = fnPartitionNameMap ?? ((i) => string.Format("{0}{1}", CollectionBase, i));

            //Build the partition cache.
            mPartitionCache = new Dictionary<int, string>();
            for (int i = 0; i < partitionCount; i++)
            {
                mPartitionCache.Add(i, mFnPartitionNameMap(i));
            }
        }
        #endregion

        #region Resolve(K key)
        /// <summary>
        /// This method resolves the incoming key in to the specific collection name.
        /// </summary>
        /// <param name="key">The key tom partition.</param>
        /// <returns>The appropriate key function.</returns>
        public virtual string Resolve(K key)
        {
            int partition = mFnPartition(key);

            if (partition < 0 || partition >= PartitionCount)
                throw new ArgumentOutOfRangeException(string.Format("ShardingPolicy: the partition function has produced an invalid value {0} for key \"{1}\" - maximum allowed is {2}", partition, key.ToString(), PartitionCount));

            return mPartitionCache[partition];
        }
        #endregion

        #region CollectionBase
        /// <summary>
        /// This is the base name for the collection.
        /// </summary>
        public string CollectionBase { get; } 
        #endregion

        #region PartitionCount
        /// <summary>
        /// This is the number of partitions for the collection.
        /// </summary>
        public int PartitionCount { get; } 
        #endregion
        #region Collections
        /// <summary>
        /// This method returns the partition collection list and is used to create the necessary documentDb collections
        /// when the persistence manager starts up.
        /// </summary>
        public virtual IEnumerable<string> Collections
        {
            get
            {
                return mPartitionCache.Values;
            }
        }
        #endregion
    }
}
