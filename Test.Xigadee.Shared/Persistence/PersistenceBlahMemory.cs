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


        public override void PrePopulate()
        {
            EntityAdd(new Blah() { ContentId = new Guid("3211c71a-24e5-474d-b35d-9e2f72cafbe8"), Message = "Hello mom.", VersionId = Guid.NewGuid() });
        }
    }
}
