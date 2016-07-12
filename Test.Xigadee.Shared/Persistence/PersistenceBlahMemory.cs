using System;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceBlahMemory: PersistenceManagerHandlerMemory<Guid, Blah>
    {
        public PersistenceBlahMemory(
              VersionPolicy<Blah> versionPolicy = null
            , ICacheManager<Guid, Blah> cacheManager = null)
            : base(
              (k) => k.ContentId
            , (s) => new Guid(s)
            , versionPolicy: versionPolicy
            //, referenceMaker: MondayMorningBluesHelper.ToReferences
            , cacheManager: cacheManager)
        {

        }
    
    }
}
