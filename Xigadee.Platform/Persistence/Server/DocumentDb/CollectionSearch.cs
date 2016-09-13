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
    /// This is the base class that holds document in the DocumentDb id.
    /// </summary>
    public class CollectionSearch: RestBase
    {
        #region Declarations

        #endregion
        #region Constructor
        public CollectionSearch(string account, string base64key, string databaseId, string collectionId, TimeSpan? defaultTimeout = null)
            : this(DocumentDbConnection.ToConnection(account, base64key), databaseId, collectionId, defaultTimeout) { }

        public CollectionSearch(DocumentDbConnection connection, string databaseId, string collectionId, TimeSpan? defaultTimeout = null)
            : base(connection, defaultTimeout)
        {
            DatabaseId = databaseId;
            CollectionId = collectionId;
            mDefaultTimeout = mDefaultTimeout ?? TimeSpan.FromSeconds(10);
        }
        #endregion

        /// <summary>
        /// This is the documentDb database name.
        /// </summary>
        public string DatabaseId { get; private set; }
        /// <summary>
        /// This is the documentDb collection name.
        /// </summary>
        public string CollectionId { get; private set; }

        #region Search(string query, int top, params SearchParameter[] parameters)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="top"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <seealso cref="https://azure.microsoft.com/en-us/documentation/articles/documentdb-sql-query/"/>
        public async Task<ResponseHolder> Search(string query, int top, params SearchParameter[] parameters)
        {
            return await Search(query, top, mDefaultTimeout, parameters);
        }

        public async Task<ResponseHolder> Search(string query, int top, TimeSpan? timeout, params SearchParameter[] parameters)
        {
            var uri = string.Format("/dbs/{0}/colls/{1}/docs", DatabaseId, CollectionId);
            var jQuery = JObject.FromObject(new { query = query, parameters = parameters.Select((i) => new { name = "@" + i.Name, value = i.Value }) });

            var json = jQuery.ToString();
            using(var content = new StringContent(json, Encoding.UTF8))
            {
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/query+json");
                return await CallClient(HttpMethod.Post, "docs", CollectionId, uri, content: content
                    , adjust: (rq) =>
                    {
                        rq.Headers.Add("x-ms-documentdb-isquery", "True");
                        rq.Headers.Add("x-ms-max-item-count", top.ToString());
                    }
                );
            }
        }
        #endregion
    }
}
