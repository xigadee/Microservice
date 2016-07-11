using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceMondayMorningBluesBlob: PersistenceMessageHandlerAzureBlobStorageBase<Guid, MondayMorningBlues>
    {
        public PersistenceMondayMorningBluesBlob(StorageCredentials credentials
            , VersionPolicy<MondayMorningBlues> versionPolicy = null
            , ICacheManager<Guid, MondayMorningBlues> cacheManager = null)
            : base(credentials, (k) => k.Id, (s) => new Guid(s), keySerializer: (g) => g.ToString("N").ToUpperInvariant(), cacheManager: cacheManager
            , versionPolicy: versionPolicy
            , referenceMaker: MondayMorningBluesHelper.ToReferences
            )
        {

        }

    }
}
