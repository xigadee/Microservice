using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceMondayMorningBluesSql: PersistenceManagerHandlerSqlBase<Guid, MondayMorningBlues>
    {
        public PersistenceMondayMorningBluesSql(string sqlConnection
            , VersionPolicy<MondayMorningBlues> versionPolicy = null
            , ICacheManager<Guid, MondayMorningBlues> cacheManager = null)
            : base(
                  sqlConnection
                  , (k) => k.Id
                  , (s) => new Guid(s)
                  , MondayMorningBluesHelper.ToMondayMorningBlues
                  , MondayMorningBluesHelper.ToXml
                  , versionPolicy: versionPolicy
                  , cacheManager: cacheManager)
        {

        }

        public override void DbSerializeKey(Guid key, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ExternalId", System.Data.SqlDbType.UniqueIdentifier).Value = key;
        }

        public override void DbSerializeEntity(MondayMorningBlues entity, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ExternalId", System.Data.SqlDbType.UniqueIdentifier).Value = entity.Id;
            cmd.Parameters.Add("@Data", SqlDbType.Xml).Value = mTransform.EntitySerializer(entity);
        }
    }
}
