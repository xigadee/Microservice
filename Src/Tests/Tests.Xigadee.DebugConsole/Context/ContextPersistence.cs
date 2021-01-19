using System;
using Xigadee;
namespace Test.Xigadee
{
    class ContextPersistence<K, E> where K : IEquatable<K>
    {
        Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> mPersistenceActivator;

        public ContextPersistence(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> persistenceActivator)
        {
            mPersistenceActivator = persistenceActivator;
            Status = 0;
        }

        public IRepositoryAsync<Guid, MondayMorningBlues> Persistence { get { return mPersistenceActivator?.Value; } }

        public int Status { get; set; }

        public bool CacheEnabled { get; set; }
    }
}
