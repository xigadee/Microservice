using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Xigadee
{
    public class BlobStorageEntityContext : EntityContext
    {
        public BlobStorageEntityContext(RepositorySettings options) : base(options)
        {

        }

        /// <summary>
        /// The entity string id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Specifies whether the blob has been soft-deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// This is the entity last update date.
        /// </summary>
        public DateTime? LastUpdated { get; set; }
        /// <summary>
        /// This is the entity Etag.
        /// </summary>
        public string ETag { get; set; }
        /// <summary>
        /// This is the entity content type
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// This is the entity content encoding type.
        /// </summary>
        public string ContentEncoding { get; set; }
        /// <summary>
        /// The safe key.
        /// </summary>
        public string SafeKey { get; set; }
        /// <summary>
        /// The directory.
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken CancelSet { get; set; }
        /// <summary>
        /// The cloud blob.
        /// </summary>
        public CloudBlockBlob Blob { get; set; }
        /// <summary>
        /// This dictionary contains the meta data fields.
        /// </summary>
        public Dictionary<string, string> Fields { get; set; }

        /// <summary>
        /// This method holds the request exception.
        /// </summary>
        public Exception Ex { get; set; }
        /// <summary>
        /// Specifies the request is successful.
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// Specifies whether the request has timed out.
        /// </summary>
        public bool IsTimeout { get; set; }
        /// <summary>
        /// This is the status code of the response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// This is the data block.
        /// </summary>
        public byte[] Data { get; set; }
    }

    public class BlobStorageEntityContext<E> : BlobStorageEntityContext, IEntityContext<E>
    {
        public BlobStorageEntityContext(RepositorySettings options, E entity) : base(options)
        {
            EntityIncoming = entity;
        }

        public List<E> ResponseEntities { get; } = new List<E>();

        public E EntityIncoming { get; }

        public E EntityOutgoing { get; set; }
    }

    public class BlobStorageEntityContext<K,E> : BlobStorageEntityContext<E>, IEntityContext<K,E>
        where K: IEquatable<K>
    {
        public BlobStorageEntityContext(RepositorySettings options, K key, E entity = default(E)) : base(options, entity)
        {
            Key = key;
        }

        public K Key { get; }
    }
}
