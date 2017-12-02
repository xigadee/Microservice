#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the interface that is used to share cached content.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The cache entity.</typeparam>
    public interface IEntityCacheAsync<K, E>: ICacheComponent
        where K : IEquatable<K>
    {
        //event EventHandler<CollectionCacheChangeEventArgs> Changed;
        /// <summary>
        /// An async method to determine if the cache contains the entity.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns true if the cache contains the key.</returns>
        Task<bool> ContainsKey(K key);
        /// <summary>
        /// This method retrieves the entity from the cache if present.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns a cache result which contains the entity if it's present in the cache.</returns>
        Task<EntityCacheResult<E>> TryGetEntity(K key);
    }

}
