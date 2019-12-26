using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public class RepositoryBlobStorageJson<K, E> : RepositoryBlobStorage<K, E>
        where K: IEquatable<K>
    {
        public RepositoryBlobStorageJson(StorageCredentials credentials
            , string containerName
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , Func<E, K> keyMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null
            , BlobRequestOptions options = null
            , OperationContext context = null
            , TimeSpan? defaultTimeout = null
            , IServiceHandlerCompression compression = null
            , IServiceHandlerEncryption encryption = null
            , IServiceHandlerSerialization serialization = null
            ):base(credentials, containerName, accessType, keyMaker, referenceMaker, propertiesMaker, versionPolicy, keyManager, options, context, defaultTimeout, compression, encryption)
        {

        }

        protected override void ContextEncodeRequest(BlobStorageEntityContext<K, E> ctx)
        {
            //Entity Serialize

            //Compress

            //Encrypt

            throw new NotImplementedException();
        }

        protected override void ContextDecodeResponse(BlobStorageEntityContext<K, E> ctx)
        {
            //Decrypt

            //Uncompress

            //Entity Deserialize

            throw new NotImplementedException();
        }


    }
}
