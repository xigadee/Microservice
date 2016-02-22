#region using
using System.Collections.Specialized;
using System.Net.Http;
using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base provider class 
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class ApiProviderBase<K, E> : ProviderBase<K, E>
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

        protected virtual void AddQueryParameters(K key, NameValueCollection queryParameters)
        {

        }

        protected virtual void LogException(string id, Exception ex)
        {

        }

        #region Constructor
        /// <summary>
        /// This is the expanded constructor where a collection of items are passed in manually.
        /// </summary>
        /// <param name="keyMaker">The key maker function.</param>
        protected ApiProviderBase(Func<E, K> keyMaker)
            : base(keyMaker)
        {
        }
        #endregion

        public override void Create(E entity, ref RepositoryOptions status)
        {
            status = status ?? new ApiRepositoryOptions();
            try
            {
                using (var client = GetClient())
                {
                    var response = client.PostAsync(FormatApiSuffix(mKeyMaker(entity), false), ContentMaker(entity)).Result;
                    var content = response.Content.ReadAsStringAsync();
                    ResponseTranslate(response, status, HttpStatusCode.Created, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                LogException(mKeyMaker(entity).ToString(), ex);
                status.ResponseCode = 500;
                status.Ex = ex;
            }
        }

        public override void Delete(K id, ref RepositoryOptions status)
        {
            status = status ?? new ApiRepositoryOptions();
            try
            {
                using (var client = GetClient())
                {
                    var response = client.DeleteAsync(FormatApiSuffix(id, false)).Result;
                    ResponseTranslate(response, status, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                LogException(id.ToString(), ex);
                status.ResponseCode = 500;
                status.Ex = ex;
            }
        }

        public override E Read(K id, ref RepositoryOptions status)
        {
            status = status ?? new ApiRepositoryOptions();
            try
            {
                using (var client = GetClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", ContentType);
                    var response = client.GetAsync(FormatApiSuffix(id, false)).Result;
                    ResponseTranslate(response, status, HttpStatusCode.OK);
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

        protected E ReadByReference(K id, string refKey, string refValue, ref RepositoryOptions status)
        {
            status = status ?? new ApiRepositoryOptions();
            try
            {
                using (var client = GetClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", ContentType);
                    var response = client.GetAsync(FormatApiSuffix(id, true, refKey, refValue)).Result;
                    ResponseTranslate(response, status, HttpStatusCode.OK);
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

        public override void Update(E entity, ref RepositoryOptions status)
        {
            status = status ?? new ApiRepositoryOptions();
            try
            {
                using (var client = GetClient())
                {
                    var response = client.PutAsync(FormatApiSuffix(mKeyMaker(entity), false), ContentMaker(entity)).Result;
                    ResponseTranslate(response, status, HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                LogException(mKeyMaker(entity).ToString(), ex);
                status.ResponseCode = 500;
                status.Ex = ex;
            }
        }

        protected abstract HttpClient GetClient();

        protected abstract string FormatApiSuffix(K key, bool isReadByRef, string refkey = "", string refValue = "");


        private void ResponseTranslate(HttpResponseMessage message, RepositoryOptions opts, params HttpStatusCode[] expectedStatusCodes)
        {
            if (expectedStatusCodes.Contains(message.StatusCode))
            {
                opts.ResponseSetOK();
                return;
            }

            var content = message.Content.ReadAsStringAsync().Result;
            var messageText = string.Empty;
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    messageText = XElement.Parse(content).Value;
                }
                catch { }
            }
            opts.ResponseCode = (int)message.StatusCode;
            opts.Ex = new Exception(string.Format("Unexpected status code returned from the API {0}. {1}", opts.ResponseCode, messageText));
        }
    }
}
