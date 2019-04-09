using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace Xigadee
{
    /// <summary>
    /// This is a more flexible API client that supports more extensible authentication and Uri mapping support.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class ApiProviderAsyncV2<K, E>: ApiProviderBase, IRepositoryAsync<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This method is used to maps entity keys to a string representation for the Uri.
        /// </summary>
        protected RepositoryKeyManager<K> mKeyMapper;
        /// <summary>
        /// This class is used to translate the requests to the appropriate Uri.
        /// </summary>
        protected TransportUriMapper<K> mUriMapper;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ApiProviderAsyncV2(Uri uri
            , RepositoryKeyManager<K> keyMapper = null
            , TransportUriMapper<K> transportUriMapper = null
            , IEnumerable<TransportSerializer> transportOverride = null
            , IApiProviderAuthBase authHandler = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
        )
            : this(uri, keyMapper, transportUriMapper, transportOverride, new[] { authHandler }, clientCert, manualCertValidation)
        {
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ApiProviderAsyncV2(Uri uri
            , RepositoryKeyManager<K> keyMapper = null
            , TransportUriMapper<K> transportUriMapper = null
            , IEnumerable<TransportSerializer> transportOverride = null
            , IEnumerable<IApiProviderAuthBase> authHandlers = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
        ) 
            :base(uri, authHandlers, clientCert, manualCertValidation, transportOverride)
        {
            mKeyMapper = keyMapper ?? RepositoryKeyManager.Resolve<K>();

            mUriMapper = transportUriMapper ?? new TransportUriMapper<K>(mKeyMapper, uri, typeof(E).Name.ToLowerInvariant());
        }
        #endregion

        #region Create(E entity, RepositorySettings options = null)
        /// <summary>
        /// This is the Create method.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="options">The optional parameters.</param>
        /// <returns>Returns the response with the entity if the request is successful.</returns>
        public virtual async Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null)
        {
            var uri = mUriMapper.MakeUri(HttpMethod.Post);

            using (var content = EntitySerialize(entity))
            {
                return await CallClient<K, E>(uri, options
                    , content: content
                    , deserializer: EntityDeserialize
                    , mapOut: (rs, holder) => ExtractHeaders(rs, holder));
            }
        }
        #endregion

        #region Read(K key, RepositorySettings options = null)
        /// <summary>
        /// This method reads an entity based on the key passed.
        /// </summary>
        /// <param name="key">The key request.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null)
        {
            var uri = mUriMapper.MakeUri(HttpMethod.Get, key);

            return await CallClient<K, E>(uri, options
                , deserializer: EntityDeserialize
                , mapOut: (rs, holder) => ExtractHeaders(rs, holder));
        }
        #endregion
        #region ReadByRef(string refKey, string refValue, RepositorySettings options = null)
        /// <summary>
        /// This method reads a reference based on its reference key and value.
        /// </summary>
        /// <param name="refKey">The key type.</param>
        /// <param name="refValue">The key value.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            var uri = mUriMapper.MakeUri(HttpMethod.Get, refKey, refValue);

            return await CallClient<K, E>(uri, options
                , deserializer: EntityDeserialize
                , mapOut: (rs, holder) => ExtractHeaders(rs, holder));
        }
        #endregion

        #region Update(E entity, RepositorySettings options = null)
        /// <summary>
        /// This method updates the entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null)
        {
            var uri = mUriMapper.MakeUri(HttpMethod.Put);

            using (var content = EntitySerialize(entity))
            {
                return await CallClient<K, E>(uri, options
                    , content: content ?? throw new ArgumentNullException("content")
                    , deserializer: EntityDeserialize
                    , mapOut: (rs, holder) => ExtractHeaders(rs, holder));
            }
        }
        #endregion

        #region Delete(K key, RepositorySettings options = null)
        /// <summary>
        /// This method deletes an entity.
        /// </summary>
        /// <param name="key">The entity unique key.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null)
        {
            var uri = mUriMapper.MakeUri(HttpMethod.Delete, key);

            return await CallClient<K, Tuple<K, string>>(uri, options
                , deserializer: EntityDeserialize
                , mapOut: (rs, holder) => ExtractHeaders(rs, holder));
        }
        #endregion
        #region DeleteByRef(string refKey, string refValue, RepositorySettings options = null)
        /// <summary>
        /// This method deletes the entity based on the reference key and value.
        /// </summary>
        /// <param name="refKey">The key type.</param>
        /// <param name="refValue">The key value.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            var uri = mUriMapper.MakeUri(HttpMethod.Delete, refKey, refValue);

            return await CallClient<K, Tuple<K, string>>(uri, options
                , deserializer: EntityDeserialize
                , mapOut: (rs, holder) => ExtractHeaders(rs, holder));
        }
        #endregion

        #region Version(K key, RepositorySettings options = null)
        /// <summary>
        /// This method resolves the version id of the entity where supported.
        /// </summary>
        /// <param name="key">The entity unique key.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null)
        {
            var uri = mUriMapper.MakeUri(HttpMethod.Head, key);

            return await CallClient<K, Tuple<K, string>>(uri, options
                , deserializer: EntityDeserialize
                , mapOut: (rs, holder) => ExtractHeaders(rs, holder));
        }
        #endregion
        #region VersionByRef(string refKey, string refValue, RepositorySettings options = null)
        /// <summary>
        /// This method resolves the entity id and version based on the reference parameters.
        /// </summary>
        /// <param name="refKey">The key type.</param>
        /// <param name="refValue">The key value.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            var uri = mUriMapper.MakeUri(HttpMethod.Head, refKey, refValue);

            return await CallClient<K, Tuple<K, string>>(uri, options
                , deserializer: EntityDeserialize
                , mapOut: (rs, holder) => ExtractHeaders(rs, holder));
        }
        #endregion

        #region Search(K key, RepositorySettings options = null)
        /// <summary>
        /// This method reads an entity based on the key passed.
        /// </summary>
        /// <param name="key">The key request.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest key, RepositorySettings options = null)
        {
            //You should not specify select parameters when selecting the full entity to be returned.
            if (string.IsNullOrEmpty(key.Select?.Trim()))
                throw new ArgumentOutOfRangeException("You must select $select parameters when selecting a SearchResponse, i.e. $select=Id, Name");

            var uri = mUriMapper.MakeUri(HttpMethod.Get, key);

            return await CallClient<SearchRequest, SearchResponse>(uri, options
                , deserializer: (r,b,rs) => rs.Entity = EntityDeserialize<SearchResponse>(r,b)
                );
        }
        #endregion
        #region SearchEntity(K key, RepositorySettings options = null)
        /// <summary>
        /// This method reads an entity based on the key passed.
        /// </summary>
        /// <param name="key">The key request.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest key, RepositorySettings options = null)
        {
            //You should not specify select parameters when selecting the full entity to be returned.
            if (!string.IsNullOrEmpty(key.Select))
                key.Select = null;

            var uri = mUriMapper.MakeUri(HttpMethod.Get, key);

            return await CallClient<SearchRequest, SearchResponse<E>>(uri, options
                , deserializer: (r, b, rs) => rs.Entity = EntityDeserialize<SearchResponse<E>>(r, b)
                );
        }
        #endregion


        #region EntityDeserialize<ET>...
        /// <summary>
        /// This method resolves the appropriate transport serializer from the incoming accept header.
        /// </summary>
        /// <param name="rs">The response</param>
        /// <param name="data">The response content</param>
        /// <param name="holder">The repository holder</param>
        /// <returns>Returns true if the serializer can be resolved.</returns>
        protected virtual void EntityDeserialize<ET>(HttpResponseMessage rs, byte[] data, RepositoryHolder<K, ET> holder)
        {
            holder.Entity = EntityDeserialize<ET>(rs, data);
        }
        #endregion

        #region ExtractHeaders...
        /// <summary>
        /// This method resolves the appropriate transport serializer from the incoming accept header.
        /// </summary>
        /// <param name="rs">The incoming response.</param>
        /// <param name="holder">The resolve transport serializer.</param>
        /// <returns>Returns true if the serializer can be resolved.</returns>
        protected virtual void ExtractHeaders(HttpResponseMessage rs, RepositoryHolder<K> holder)
        {
            string contentId = rs.Headers.Where((h) => h.Key.Equals("x-entityid", StringComparison.InvariantCultureIgnoreCase))
                .SelectMany((h) => h.Value).FirstOrDefault() ?? "";

            string versionId = rs.Headers.Where((h) => h.Key.Equals("x-versionid", StringComparison.InvariantCultureIgnoreCase))
                .SelectMany((h) => h.Value).FirstOrDefault() ?? "";

            holder.KeyReference = new Tuple<string, string>(contentId, versionId);

            if (!string.IsNullOrEmpty(contentId))
                holder.Key = mKeyMapper.Deserialize(contentId);
        }
        #endregion

        #region RequestHeadersSetTransport(HttpRequestMessage rq)
        /// <summary>
        /// This method sets the media quality type for the entity transfer.
        /// </summary>
        /// <param name="rq">The http request.</param>
        protected override void RequestHeadersSetTransport(HttpRequestMessage rq)
        {
            mTransportSerializers.ForEach((t) => rq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(t.Value.MediaType, t.Value.Priority)));
        }
        #endregion

        #region UserAgentGet()
        /// <summary>
        /// This method returns the user agent that is passed to the calling party. 
        /// </summary>
        /// <returns>Returns a string containing the user agent.</returns>
        protected override string UserAgentGet()
        {
            return $"{base.UserAgentGet()} {typeof(E).Name}";
        }
        #endregion
    }
}
