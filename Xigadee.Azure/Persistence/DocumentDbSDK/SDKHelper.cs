using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Xigadee
{
    public static class DocumentDbSDKHelper
    {

        public static DocumentClient ToDocumentClient(this DocumentDbConnection conn, ConnectionPolicy policy = null, ConsistencyLevel? level = null)
        {
            return new DocumentClient(conn.Account, conn.AccountKey, policy??ConnectionPolicy.Default, level);
        }


        public static async Task<ResponseHolder> CreateGeneric(this DocumentClient client, string json, string nameDatabase, string nameCollection)
        {

            return await DocDbExceptionWrapper(async e => 
            {

                ResourceResponse<Document> resultDocDb = null;
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var doc = Document.LoadFrom<Document>(ms);
                    var uri = UriFactory.CreateDocumentCollectionUri(nameDatabase, nameCollection);
                    resultDocDb = await client.CreateDocumentAsync(uri, doc, disableAutomaticIdGeneration: true);
                }

                e.IsSuccess = true;
                e.StatusCode = (int)resultDocDb.StatusCode;
                e.Content = resultDocDb.Resource.ToString();
                e.ETag = resultDocDb.Resource.ETag;
            }
            );

        }

        public static async Task<ResponseHolder> ReadGeneric(this DocumentClient client, string nameDatabase, string nameCollection, string nameId)
        {
            return await DocDbExceptionWrapper(async e =>
            {
                var uri = UriFactory.CreateDocumentUri(nameDatabase, nameCollection, nameId);

                ResourceResponse<Document> resultDocDb = await client.ReadDocumentAsync(uri);

                e.IsSuccess = true;
                e.StatusCode = (int)resultDocDb.StatusCode;
                e.Content = resultDocDb.Resource.ToString();
                e.ETag = resultDocDb.Resource.ETag;
            });
        }

        public static async Task<ResponseHolder> DeleteGeneric(this DocumentClient client, string nameDatabase, string nameCollection, string nameId)
        {
            return await DocDbExceptionWrapper(async e =>
            {
                var uri = UriFactory.CreateDocumentUri(nameDatabase, nameCollection, nameId);

                ResourceResponse<Document> resultDocDb = await client.DeleteDocumentAsync(uri);

                e.IsSuccess = true;
                e.StatusCode = (int)resultDocDb.StatusCode;
            });
        }

        private static async Task<ResponseHolder> DocDbExceptionWrapper(Func<ResponseHolder, Task> action)
        {
            var response = new ResponseHolder();

            try
            {
                await action(response);
            }
            catch (DocumentClientException dex)
            {
                response.StatusCode = (int)dex.StatusCode;
                response.IsSuccess = false;
            }
            catch (Exception ex)
            {
                response.Ex = ex;
                response.StatusCode = 500;
                response.IsSuccess = false;
            }

            return response;
        }
    }
}
