using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xigadee;

namespace Test.Xigadee.Api.Server
{
    public class ProviderBlahAsyncTransmit: CorePersistenceMessageInitiator<Guid, Blah>
    {
    }
    public class ProviderMondayMorningBluesAsyncTransmit: CorePersistenceMessageInitiator<Guid, MondayMorningBlues>
    {
    }
    public class ProviderComplexEntityAsyncTransmit: CorePersistenceMessageInitiator<ComplexKey, ComplexEntity>
    {
    }

    public class CorePersistenceMessageInitiator<K, E>: PersistenceMessageInitiator<K, E> where K : IEquatable<K>
    {
        public CorePersistenceMessageInitiator()
        {
            ChannelId = CoreChannels.RequestCore;
            ResponseChannelId = CoreChannels.ResponseBff;
        }
    }

}