using System;
using System.Collections;
using System.Collections.Generic;
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

        public override void DbSerializeEntity(MondayMorningBlues entity, SqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public override void DbSerializeKey(Guid key, SqlCommand cmd)
        {
            throw new NotImplementedException();
        }
    }
}
