using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    //public class PersistenceRedisMondayMorningBlues: PersistenceMessageHandlerRedisCache<Guid, MondayMorningBlues>
    //{
    //    public PersistenceRedisMondayMorningBlues(string redisConnection, VersionPolicy<MondayMorningBlues> versionPolicy = null)
    //        : base(redisConnection, (k) => k.Id, (s) => new Guid(s), versionPolicy: versionPolicy, referenceMaker: References)
    //    {

    //    }

    //    static IEnumerable<Tuple<string, string>> References(MondayMorningBlues entity)
    //    {
    //        if (entity != null && !string.IsNullOrEmpty(entity.Email))
    //            return new[] { new Tuple<string, string>("email", entity.Email) };

    //        return new Tuple<string, string>[] { };
    //    }
    //}
}
