#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
