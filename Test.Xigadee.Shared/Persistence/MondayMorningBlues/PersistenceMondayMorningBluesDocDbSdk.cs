using System;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceMondayMorningBluesDocDbSdk: PersistenceMessageHandlerDocumentDbSdk<Guid, MondayMorningBlues, PersistenceStatistics, PersistenceCommandPolicy>
    {
        public PersistenceMondayMorningBluesDocDbSdk(DocumentDbConnection connection, string name
            , VersionPolicy<MondayMorningBlues> versionPolicy = null
            , ICacheManager<Guid, MondayMorningBlues> cacheManager = null)
            : base(connection, name, (k) => k.Id, (s) => new Guid(s)
            , versionPolicy: versionPolicy
            , referenceMaker: MondayMorningBluesHelper.ToReferences
            , cacheManager: cacheManager)
        {

        }

    }
}
