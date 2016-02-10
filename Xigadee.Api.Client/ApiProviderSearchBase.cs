
using System.Net.Http;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Xigadee.Api
{
    public abstract class ApiProviderSearchBase<K, E> : ProviderBase<K, E>
        where K : IEquatable<K>
        where E : class
    {
        protected abstract string ApiSuffix { get; }

        protected abstract E EntityMaker(HttpContent data);

        protected abstract HttpContent ContentMaker(E entity);

        protected virtual string ContentType { get { return "application/xml"; } }

        protected virtual string UriKeyMaker(K key)
        {
            return key.ToString();
        }

        protected virtual void LogException(string id, Exception ex)
        {

        }

        #region Constructor
        /// <summary>
        /// This is the expanded constructor where a collection of items are passed in manually.
        /// </summary>
        /// <param name="keyMaker">The key maker function.</param>
        protected ApiProviderSearchBase(Func<E, K> keyMaker)
            : base(keyMaker)
        {
        }
        #endregion

        public override E Read(K id, ref RepositoryOptions status)
        {
            status = status ?? new ApiRepositoryOptions();
            try
            {
                using (var client = GetClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", ContentType);
                    var response = client.GetAsync(FormatApiSuffix(id, false)).Result;
                    ResponseTranslate(response.StatusCode, status, HttpStatusCode.OK);
                    if (response.IsSuccessStatusCode)
                        return EntityMaker(response.Content);
                }
            }
            catch (Exception ex)
            {
                LogException(id.ToString(), ex);
                status.ResponseCode = 500;
                status.Ex = ex;
            }

            return null;
        }


        protected abstract HttpClient GetClient();

        protected abstract string FormatApiSuffix(K key, bool isReadByRef, string refkey = "");

        private void ResponseTranslate(HttpStatusCode statusCode, RepositoryOptions opts, params HttpStatusCode[] expectedStatusCodes)
        {
            if (expectedStatusCodes.Contains(statusCode))
            {
                opts.ResponseSetOK();
                return;
            }

            opts.ResponseCode = (int)statusCode;
            opts.Ex = new Exception("Unexpected status code returned from the API " + opts.ResponseCode);
        }
    }
}
