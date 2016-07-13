using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xigadee;

namespace Test.Xigadee.Api.Server
{

    public class ProviderBlahAsyncLocal: CorePersistenceSharedService<Guid, Blah>
    {
    }

    public class ProviderMondayMorningBluesAsyncLocal: CorePersistenceSharedService<Guid, MondayMorningBlues>
    {
    }

    public class ProviderComplexEntityAsyncLocal: CorePersistenceSharedService<ComplexKey, ComplexEntity>
    {
    }

    public class CorePersistenceSharedService<K, E>: PersistenceSharedService<K, E>
        where K : IEquatable<K>
    {
        public CorePersistenceSharedService():base(responseChannel: CoreChannels.Internal)
        {

        }
    }
}