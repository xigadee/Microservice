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

using System;
using System.Collections.Generic;

namespace Xigadee
{
    #region IResponseHolder<E>
    /// <summary>
    /// This is the generic response holder interface that contains the entity
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IResponseHolder<E> : IResponseHolder
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        E Entity { get; }
    }
    #endregion
    #region IResponseHolder
    /// <summary>
    /// The root response holder interface.
    /// </summary>
    public interface IResponseHolder
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        string Content { get; set; }
        /// <summary>
        /// Gets or sets any exception raised.
        /// </summary>
        Exception Ex { get; set; }
        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        Dictionary<string, string> Fields { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Gets or sets the version identifier.
        /// </summary>
        string VersionId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is successful.
        /// </summary>
        bool IsSuccess { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is timed out.
        /// </summary>
        bool IsTimeout { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is a cache hit.
        /// </summary>
        bool IsCacheHit { get; set; }
        /// <summary>
        /// Gets the status code.
        /// </summary>
        int StatusCode { get; }
    } 
    #endregion
}