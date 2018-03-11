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
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This method encapsulates the basic DocumentDb account functionality.
    /// </summary>
    public class Account:RestBase
    {
        public Account(string account, string base64key, TimeSpan? defaultTimeout = null)
            : this(DocumentDbConnection.ToConnection(account, base64key), defaultTimeout){}

        public Account(DocumentDbConnection connection, TimeSpan? defaultTimeout = null):base(connection, defaultTimeout)
        {
            Name = connection.AccountName;
            mDefaultTimeout = mDefaultTimeout ?? TimeSpan.FromSeconds(30);
        }

        public async Task<ResponseHolder<Database>> Create(string databaseName)
        {
            var jQuery = JObject.FromObject(new { id = databaseName });
            var json = jQuery.ToString();

            var uri = "/dbs";
            ResponseHolder<Database> rs;

            using (var content = new StringContent(json, Encoding.UTF8))
            {
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                rs = await CallClient<ResponseHolder<Database>>(HttpMethod.Post, "dbs", "", uri, content: content
                    , timeout: mDefaultTimeout ?? TimeSpan.FromSeconds(30));
            }

            if (rs != null && rs.IsSuccess)
            {
                var rsjQuery = JObject.Parse(rs.Content);
                string databaseId = rsjQuery["_rid"].Value<string>();
                rs.Entity = new Database(Connection, databaseId, mDefaultTimeout);
            }

            return rs;
        }

        public async Task<ResponseHolder> List(int top = 100)
        {
            var uri = "/dbs";
            return await CallClient(HttpMethod.Get, "dbs", "", uri,
                adjust: (rq) => rq.Headers.Add("x-ms-max-item-count", top.ToString())
                , timeout: mDefaultTimeout??TimeSpan.FromSeconds(30));
        }

        public async Task<Database> Resolve(string databaseName, TimeSpan? defaultTimeout = null)
        {
            var response = await List();

            if (!response.IsSuccess)
                return null;

            var dbs = JObject.Parse(response.Content);

            var database = dbs["Databases"].Values<JObject>()
                .Where(n => n["id"].Value<string>() == databaseName)
                .Select((i) => i["_rid"].Value<string>())
                .FirstOrDefault();

            if (database == null)
                return null;

            return new Database(Connection, database, defaultTimeout??mDefaultTimeout) { Name = databaseName };
        }
    }
}
