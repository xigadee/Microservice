using System;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceMondayMorningBluesMemory: PersistenceManagerHandlerMemory<Guid, MondayMorningBlues>
    {
        public PersistenceMondayMorningBluesMemory(
              VersionPolicy<MondayMorningBlues> versionPolicy = null
            , ICacheManager<Guid, MondayMorningBlues> cacheManager = null)
            : base(
              (k) => k.Id
            , (s) => new Guid(s)
            , versionPolicy: versionPolicy
            , referenceMaker: MondayMorningBluesHelper.ToReferences
            , cacheManager: cacheManager)
        {

        }

    }
}
