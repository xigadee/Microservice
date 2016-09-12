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

#region using
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This method encapsulates the database concept in DocumentDb.
    /// </summary>
    public class Database : RestBase
    {


        public Database(string account, string base64key, string databaseId, TimeSpan? defaultTimeout = null)
            : this(DocumentDbConnection.ToConnection(account, base64key), databaseId, defaultTimeout) { }

        public Database(DocumentDbConnection connection, string databaseId, TimeSpan? defaultTimeout = null)
            : base(connection, defaultTimeout)
        {
            DatabaseId = databaseId;
            mDefaultTimeout = mDefaultTimeout ?? TimeSpan.FromSeconds(20);
        }

        public string DatabaseId { get; private set; }

        public async Task<ResponseHolder<Collection>> Create(string collectionName, string databaseName)
        {
            var jQuery = JObject.FromObject(new { id = collectionName });
            var json = jQuery.ToString();

            var uri = string.Format("/dbs/{0}/colls", DatabaseId);
            ResponseHolder<Collection> rs;

            using (var content = new StringContent(json, Encoding.UTF8))
            {
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                rs =  await CallClient<ResponseHolder<Collection>>(HttpMethod.Post, "colls", DatabaseId, uri, content: content
                    , timeout:TimeSpan.FromSeconds(30));
            }

            if (rs != null && rs.IsSuccess && rs.Response.IsSuccessStatusCode)
            {
                var rsjQuery = JObject.Parse(rs.Content);
                string collectionId = rsjQuery["_rid"].Value<string>();
                rs.Entity = new Collection(Connection, DatabaseId, collectionId, mDefaultTimeout) { Name = collectionName, DatabaseName = databaseName };
            }

            return rs;
        }

        public async Task<ResponseHolder> Read(string collectionId)
        {
            var uri = string.Format("/dbs/{0}/colls/{1}", DatabaseId, collectionId);
            return await CallClient(HttpMethod.Get, "colls", collectionId, uri);
        }

        //public async Task<ResponseHolder<Collection>> DeleteByRef(string collectionName)
        //{
        //}

        public async Task<ResponseHolder> Delete(string collectionId)
        {
            var uri = string.Format("/dbs/{0}/colls/{1}", DatabaseId, collectionId);
            return await CallClient(HttpMethod.Delete, "colls", collectionId, uri);
        }

        public async Task<ResponseHolder> List(int top = 100)
        {
            var uri = string.Format("/dbs/{0}/colls", DatabaseId);
            return await CallClient(HttpMethod.Get, "colls", DatabaseId, uri, 
                adjust:(rq)=>rq.Headers.Add("x-ms-max-item-count", top.ToString())
                , timeout:TimeSpan.FromSeconds(30));
        }

        public async Task<Collection> Resolve(string collectionName, string databaseName, TimeSpan? defaultTimeout = null)
        {
            var response = await List();
            if (!response.IsSuccess)
                return null;

            var dbs = JObject.Parse(response.Content);
            var collectionId = dbs["DocumentCollections"].Values<JObject>()
                .Where(n => n["id"].Value<string>() == collectionName)
                .Select((i) => i["_rid"].Value<string>())
                .FirstOrDefault();

            if (collectionId == null)
                return null;

            return new Collection(Connection, DatabaseId, collectionId, defaultTimeout ?? mDefaultTimeout) { Name = collectionName, DatabaseName = databaseName };
        }
    }
}
