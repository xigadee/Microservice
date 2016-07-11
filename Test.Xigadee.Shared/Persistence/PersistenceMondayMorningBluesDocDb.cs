using System;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceMondayMorningBluesDocDb: PersistenceManagerHandlerDocumentDb<Guid, MondayMorningBlues>
    {
        public PersistenceMondayMorningBluesDocDb(DocumentDbConnection connection, string name
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
