using System;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceMondayMorningBluesRedis: PersistenceMessageHandlerRedisCache<Guid, MondayMorningBlues>
    {
        public PersistenceMondayMorningBluesRedis(string connection
            , VersionPolicy<MondayMorningBlues> versionPolicy = null
            , ICacheManager<Guid, MondayMorningBlues> cacheManager = null)
            : base(connection, (k) => k.Id, (s) => new Guid(s)
            , versionPolicy: versionPolicy
            , referenceMaker: References)
        {

        }

        static IEnumerable<Tuple<string, string>> References(MondayMorningBlues entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.Email))
                return new[] { new Tuple<string, string>("email", entity.Email) };

            return new Tuple<string, string>[] { };
        }
    }
}
