using System;
using System.Collections.Generic;
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
                  , new EntitySerializer<MondayMorningBlues>(MondayMorningBluesHelper.ToXml, MondayMorningBluesHelper.ToMondayMorningBlues)
                  , xmlVersionMaker: MondayMorningBluesHelper.ToVersion
                  , versionPolicy: versionPolicy
                  , cacheManager: cacheManager
                  , referenceMaker: ReferenceMaker)
        {

        }

        #region ReferenceMaker
        static IEnumerable<Tuple<string, string>> ReferenceMaker(MondayMorningBlues mondayMorningBlues)
        {
            return new List<Tuple<string,string>>{ new Tuple<string,string> ("EMAIL", mondayMorningBlues.Email) };
        }
        #endregion

        public override void DbSerializeKey(Guid key, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ExternalId", SqlDbType.UniqueIdentifier).Value = key;
        }

        public override void DbSerializeEntity(MondayMorningBlues entity, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ExternalId", SqlDbType.UniqueIdentifier).Value = entity.Id;
            cmd.Parameters.Add("@Data", SqlDbType.Xml).Value = mTransform.PersistenceEntitySerializer.Serializer(entity);
        }
    }
}
