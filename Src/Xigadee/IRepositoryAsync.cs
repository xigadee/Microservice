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
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is a default repository async interface for entities within the system.
    /// </summary>
    /// <typeparam name="K">The entity key object.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IRepositoryAsyncServer<K, E>: IRepositoryAsync<K, E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// Gets or sets the default security principal.
        /// </summary>
        IPrincipal DefaultPrincipal { get; set; }
    }

    /// <summary>
    /// This is a default repository async interface for entities within the system.
    /// </summary>
    /// <typeparam name="K">The entity key object.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IRepositoryAsync<K, E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null);
        /// <summary>
        /// Reads the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null);
        /// <summary>
        /// Reads the entity by a reference key-value pair.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null);
        /// <summary>
        /// Deletes the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null);
        /// <summary>
        /// Deletes the entity by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Validates the version by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null);
        /// <summary>
        /// Validates the version by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest key, RepositorySettings options = null);    
    }
}
