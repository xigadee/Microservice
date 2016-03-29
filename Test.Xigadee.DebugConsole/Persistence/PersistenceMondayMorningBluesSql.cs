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
        public PersistenceMondayMorningBluesSql(string sqlConnection)
            : base(sqlConnection, (k) => k.Id, (x) => new MondayMorningBlues())
        {

        }

        static IEnumerable<Tuple<string, string>> References(MondayMorningBlues entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.Email))
                return new[] { new Tuple<string, string>("email", entity.Email) };

            return new Tuple<string, string>[] { };
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
