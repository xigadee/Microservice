#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class contains the majority of DocumentDb collection functionality.
    /// </summary>
    public class Collection : RestBase
    {
        #region Declarations
  
        #endregion
        #region Constructor
        public Collection(string account, string base64key, string databaseId, string collectionId, TimeSpan? defaultTimeout = null)
            : this(DocumentDbConnection.ToConnection(account, base64key), databaseId, collectionId, defaultTimeout) { }

        public Collection(DocumentDbConnection connection, string databaseId, string collectionId, TimeSpan? defaultTimeout = null)
            : base(connection, defaultTimeout)
        {
            DatabaseId = databaseId;
            CollectionId = collectionId;
            mDefaultTimeout = mDefaultTimeout ?? TimeSpan.FromSeconds(10);
        }
        #endregion

        #region DatabaseName
        /// <summary>
        /// This is the public database name.
        /// </summary>
        public string DatabaseName { get; set; }
        #endregion
        #region DatabaseId
        /// <summary>
        /// This is the documentDb database name.
        /// </summary>
        public string DatabaseId { get; private set; }
        #endregion
        #region CollectionId
        /// <summary>
        /// This is the documentDb collection name.
        /// </summary>
        public string CollectionId { get; private set; } 
        #endregion

        #region Create(string json, TimeSpan? timeout = null)
        /// <summary>
        /// This method creates the entity of the documentDb collection
        /// </summary>
        /// <param name="json">The json body.</param>
        /// <param name="timeout">The time out period.</param>
        /// <returns>Returns the response holder.</returns>
        public async Task<ResponseHolder> Create(string json, TimeSpan? timeout = null)
        {
            var uri = string.Format("/dbs/{0}/colls/{1}/docs", DatabaseId, CollectionId);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                return await CallClient(HttpMethod.Post, "docs", CollectionId, uri, content: content, timeout: timeout);
            }
        }
        #endregion

        #region Read(string documentId, TimeSpan? timeout = null)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<ResponseHolder> Read(string documentId, TimeSpan? timeout = null)
        {
            var uri = string.Format("/dbs/{0}/colls/{1}/docs/{2}", DatabaseId, CollectionId, documentId);
            return await CallClient(HttpMethod.Get, "docs", documentId, uri, timeout: timeout);
        }
        #endregion
        #region ReadDirect(string documentId, TimeSpan? timeout = null)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<ResponseHolder> ReadDirect(string id, TimeSpan? timeout = null)
        {
            var uri = string.Format("dbs/{0}/colls/{1}/docs/{2}", DatabaseName, Name, id);
            return await CallClient(HttpMethod.Get, "docs", uri, uri, timeout: timeout, idBased:true);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeout"></param>
        /// <param name="extractionJPaths"></param>
        /// <returns></returns>
        public async Task<ResponseHolder> ResolveDocumentId(string id, TimeSpan? timeout = null
            , IEnumerable<KeyValuePair<string, string>> extractionJPaths = null)
        {
            return await ResolveDocumentId("id", id, timeout: timeout, extractionJPaths: extractionJPaths);
        }

        /// <summary>
        /// This method resolves the documentId, and etag for the document in DocumentDb
        /// </summary>
        /// <param name="field"></param>
        /// <param name="id">The entity Id.</param>
        /// <param name="timeout"></param>
        /// <param name="extractionJPaths"></param>
        /// <returns>Returns a response holder that holds teh data.</returns>
        public async Task<ResponseHolder> ResolveDocumentId(string field, string id, TimeSpan? timeout = null
            , IEnumerable<KeyValuePair<string, string>> extractionJPaths = null)
        {
            var sql = $"SELECT * FROM c WHERE (c.{field}=@id)";
            var param = new SearchParameter() { Name = "id", Value = id};
            var search = await Search(sql, 1, timeout, null, null, param);

            var holder = new ResponseHolder {IsTimeout = search.IsTimeout};

            if (search.IsSuccess)
            {
                var jQuery = JObject.Parse(search.Content);
                int count = jQuery["_count"].Value<int>();

                if (count == 1)
                {
                    JToken entity = jQuery["Documents"].First();

                    holder.Id = id;
                    holder.IsSuccess = true;
                    holder.DocumentId = entity["_rid"].Value<string>();
                    holder.ETag = entity["_etag"].Value<string>();
                    holder.Content = entity.ToString();

                    //Extract the additional fields specified in the extraction collection.
                    if (extractionJPaths != null)
                        foreach (var jpath in extractionJPaths)
                        {
                            var token = entity[jpath.Value];
                            if (token != null)
                                holder.Fields.Add(jpath.Key, token.Value<string>());
                        }
                }
                else
                {
                    holder.IsSuccess = false;
                    holder.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }
            else
            {
                holder.Ex = search.Ex;
                holder.Response = search.Response;
            }


            return holder;
        }

        #region Update(string documentId, string json, TimeSpan? timeout = null, string eTag = null)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="json"></param>
        /// <param name="timeout"></param>
        /// <param name="eTag"></param>
        /// <returns></returns>
        public async Task<ResponseHolder> Update(string documentId, string json, TimeSpan? timeout = null, string eTag = null)
        {
            var uri = string.Format("/dbs/{0}/colls/{1}/docs/{2}", DatabaseId, CollectionId, documentId);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                return await CallClient(HttpMethod.Put, "docs", documentId, uri, content: content, timeout: timeout, eTag: eTag);
            }
        }
        #endregion
        #region Delete(string documentId, TimeSpan? timeout = null, string eTag = null)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="timeout"></param>
        /// <param name="eTag"></param>
        /// <returns></returns>
        public async Task<ResponseHolder> Delete(string documentId, TimeSpan? timeout = null, string eTag = null)
        {
            var uri = string.Format("/dbs/{0}/colls/{1}/docs/{2}", DatabaseId, CollectionId, documentId);
            return await CallClient(HttpMethod.Delete, "docs", documentId, uri, timeout: timeout, eTag:eTag);
        }
        #endregion        
        #region DeleteDirect(string documentId, TimeSpan? timeout = null, string eTag = null)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="timeout"></param>
        /// <param name="eTag"></param>
        /// <returns></returns>
        public async Task<ResponseHolder> DeleteDirect(string id, TimeSpan? timeout = null, string eTag = null)
        {
            var uri = string.Format("dbs/{0}/colls/{1}/docs/{2}", DatabaseName, Name, id);
            return await CallClient(HttpMethod.Delete, "docs", id, uri, timeout: timeout, idBased: true, eTag:eTag);
        }
        #endregion

        #region List(int top = 100, TimeSpan? timeout = null)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="top"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<ResponseHolder> List(int top = 100, TimeSpan? timeout = null)
        {
            var uri = string.Format("/dbs/{0}/colls/{1}/docs", DatabaseId, CollectionId);
            return await CallClient(HttpMethod.Get, "docs", CollectionId, uri,
                adjust: (rq) => rq.Headers.Add("x-ms-max-item-count", top.ToString()), timeout: timeout);
        } 
        #endregion

        #region Search(string query, int top, params SearchParameter[] parameters)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="top"></param>
        /// <param name="sessionToken"></param>
        /// <param name="continuationToken"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <seealso cref="https://azure.microsoft.com/en-us/documentation/articles/documentdb-sql-query/"/>
        public async Task<ResponseHolder> Search(string query, int top, string sessionToken = null, string continuationToken = null, params SearchParameter[] parameters)
        {
            return await Search(query, top, mDefaultTimeout, sessionToken, continuationToken, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="top"></param>
        /// <param name="timeout"></param>
        /// <param name="sessionToken"></param>
        /// <param name="continuationToken"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<ResponseHolder> Search(string query, int top, TimeSpan? timeout, string sessionToken = null, string continuationToken = null, params SearchParameter[] parameters)
        {
            var uri = $"/dbs/{DatabaseId}/colls/{CollectionId}/docs";
            var jQuery = JObject.FromObject(new {query, parameters = parameters.Select(i => new { name = "@" + i.Name, value = i.Value }) });

            var json = jQuery.ToString();
            using (var content = new StringContent(json, Encoding.UTF8))
            {
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/query+json");
                return await CallClient(HttpMethod.Post, "docs", CollectionId, uri, content: content
                    , adjust: (rq) =>
                        {
                            rq.Headers.Add("x-ms-documentdb-isquery", "True");
                            rq.Headers.Add("x-ms-max-item-count", top.ToString());
                            if (!string.IsNullOrEmpty(sessionToken))
                                rq.Headers.Add("x-ms-session-token", sessionToken);
                            if (!string.IsNullOrEmpty(continuationToken))
                                rq.Headers.Add("x-ms-continuation", continuationToken);
                        }
                    , timeout: timeout
                );
            }
        } 
        #endregion

    }
}
