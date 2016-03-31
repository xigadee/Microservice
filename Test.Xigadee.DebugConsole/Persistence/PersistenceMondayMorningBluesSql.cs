using System;
using System.Data;
using System.Data.SqlClient;
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
                  , MondayMorningBluesHelper.ToVersion
                  , versionPolicy: versionPolicy
                  , cacheManager: cacheManager)
        {

        }

        public override void DbSerializeKey(Guid key, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ExternalId", SqlDbType.UniqueIdentifier).Value = key;
        }

        public override void DbSerializeEntity(MondayMorningBlues entity, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ExternalId", SqlDbType.UniqueIdentifier).Value = entity.Id;
            cmd.Parameters.Add("@Data", SqlDbType.Xml).Value = mTransform.EntitySerializer(entity);
        }
    }
}
