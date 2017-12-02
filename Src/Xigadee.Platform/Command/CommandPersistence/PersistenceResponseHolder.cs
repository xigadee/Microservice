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
    #region PersistenceResponseHolder<E>
    /// <summary>
    /// This is the generic class is used to return an entity and its status.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <seealso cref="Xigadee.PersistenceResponseHolder" />
    /// <seealso cref="Xigadee.IResponseHolder{E}" />
    public class PersistenceResponseHolder<E> : PersistenceResponseHolder, IResponseHolder<E>
    {
        #region Constructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceResponseHolder{E}"/> class.
        /// </summary>
        public PersistenceResponseHolder() : base()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceResponseHolder{E}"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="content">The content.</param>
        /// <param name="entity">The entity.</param>
        public PersistenceResponseHolder(PersistenceResponse? status = null, string content = null, E entity = default(E)) : base(status, content)
        {
            Entity = entity;
        }
        #endregion

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        public E Entity
        {
            get; set;
        }
    }
    #endregion

    #region PersistenceResponseHolder
    /// <summary>
    /// This class is used to hold a response to a request that does not return an entity to the calling client, i.e. Delete, Version etc.
    /// </summary>
    /// <seealso cref="Xigadee.PersistenceResponseHolder" />
    /// <seealso cref="Xigadee.IResponseHolder{E}" />
    public class PersistenceResponseHolder : IResponseHolder
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public PersistenceResponseHolder()
        {
            Fields = new Dictionary<string, string>();
        }
        /// <summary>
        /// This is the extended constructor.
        /// </summary>
        /// <param name="status">The response status.</param>
        /// <param name="content">The context as a string.</param>
        public PersistenceResponseHolder(PersistenceResponse? status = null, string content = null) : this()
        {
            if (status.HasValue)
            {
                StatusCode = (int)status.Value;
                IsSuccess = StatusCode >= 200 && StatusCode <= 299;

                IsTimeout = !IsSuccess && StatusCode == 408;
            }
            Content = content;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the exception value.
        /// </summary>
        public Exception Ex
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the metadata fields.
        /// </summary>
        public Dictionary<string, string> Fields
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this request is successful.
        /// </summary>
        public bool IsSuccess
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this request has timed out.
        /// </summary>
        public bool IsTimeout
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this request is a cache hit.
        /// </summary>
        public bool IsCacheHit
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int StatusCode
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets the version identifier of the entity.
        /// </summary>
        public string VersionId
        {
            get; set;
        }

    } 
    #endregion
}
