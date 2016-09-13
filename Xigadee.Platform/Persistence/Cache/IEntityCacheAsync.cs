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

        Task<bool> ContainsKey(K key);

        Task<EntityCacheResult<E>> TryGetEntity(K key);
    }

}
