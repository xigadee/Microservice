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
    public class ApiProviderAsyncV2<K, E>: ApiProviderBase, IRepositoryAsync<K, E>, IRepositoryBase//, IRepositoryAsyncServer<K, E>
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
            MappersSet(keyMapper, transportUriMapper);
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ApiProviderAsyncV2(ConnectionContext context
            , RepositoryKeyManager<K> keyMapper = null
            , TransportUriMapper<K> transportUriMapper = null
        )
            : base(context)
        {
            MappersSet(keyMapper, transportUriMapper);
        }
        #endregion

        /// <summary>
        /// This method sets the default mappers for the application.
        /// </summary>
        /// <param name="keyMapper">The entity key mapper.</param>
        /// <param name="transportUriMapper">The entity transport mapper.</param>
        protected virtual void MappersSet(RepositoryKeyManager<K> keyMapper, TransportUriMapper<K> transportUriMapper)
        {
            mKeyMapper = keyMapper ?? DefaultKeyMapper();

            mUriMapper = transportUriMapper ?? DefaultTransportUriMapper();
        }
        /// <summary>
        /// This method returns the default key mapper for the entity key.
        /// </summary>
        /// <returns>Returns an instance of the mapper.</returns>
        protected virtual RepositoryKeyManager<K> DefaultKeyMapper() => RepositoryKeyManager.Resolve<K>();
        /// <summary>
        /// This method sets the default transport Uri Mapper.
        /// </summary>
        /// <returns>Returns the transport mapper.</returns>
        protected virtual TransportUriMapper<K> DefaultTransportUriMapper() 
            => new TransportUriMapper<K>(mKeyMapper, Context.Uri, typeof(E).Name.ToLowerInvariant());

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
                key.AssignSelect(null);

            var uri = mUriMapper.MakeUri(HttpMethod.Get, key);

            return await CallClient<SearchRequest, SearchResponse<E>>(uri, options
                , deserializer: (r, b, rs) => rs.Entity = EntityDeserialize<SearchResponse<E>>(r, b)
                );
        }
        #endregion

        #region History(HistoryRequest<K> key, RepositorySettings options = null)
        /// <summary>
        /// This method reads an entity based on the key passed.
        /// </summary>
        /// <param name="key">The key request.</param>
        /// <param name="options">These are the repository options which define the request type..</param>
        /// <returns>This is the holder containing the response and the entity where necessary.</returns>
        public virtual async Task<RepositoryHolder<HistoryRequest<K>, HistoryResponse<E>>> History(HistoryRequest<K> key, RepositorySettings options = null)
        {
            //You should not specify select parameters when selecting the full entity to be returned.
            if (!string.IsNullOrEmpty(key.Select))
                key.AssignSelect(null);

            var uri = mUriMapper.MakeUri(HttpMethod.Get, key);

            return await CallClient<HistoryRequest<K>, HistoryResponse<E>>(uri, options
                , deserializer: (r, b, rs) => rs.Entity = EntityDeserialize<HistoryResponse<E>>(r, b)
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
            Context.TransportSerializers.ForEach((t) => rq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(t.Value.MediaType, t.Value.Priority)));
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

        #region CallClient<KT,ET>...
        /// <summary>
        /// This method calls the client using HTTP and returns the response along with the entity in the response if supplied.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="uri">The request Uri.</param>
        /// <param name="options">The repository settings passed from the caller.</param>
        /// <param name="content">The HttpContent to send to the API.</param>
        /// <param name="adjustIn">Any message adjustment.</param>
        /// <param name="mapOut">Any response adjustment before returning to the caller.</param>
        /// <param name="deserializer">Deserialize the response content into the entity</param>
        /// <returns>Returns the repository holder.</returns>
        protected virtual Task<RepositoryHolder<KT, ET>> CallClient<KT, ET>(
              KeyValuePair<HttpMethod, Uri> uri
            , RepositorySettings options
            , HttpContent content = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpResponseMessage, RepositoryHolder<KT, ET>> mapOut = null
            , Action<HttpResponseMessage, byte[], RepositoryHolder<KT, ET>> deserializer = null) =>
            CallClient(uri.Key, uri.Value, options, content, adjustIn, mapOut, deserializer);

        /// <summary>
        /// This method calls the client using HTTP and returns the response along with the entity in the response if supplied.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The request Uri.</param>
        /// <param name="options">The repository settings passed from the caller.</param>
        /// <param name="content">The HttpContent to send to the API.</param>
        /// <param name="adjustIn">Any message adjustment.</param>
        /// <param name="mapOut">Any response adjustment before returning to the caller.</param>
        /// <param name="deserializer">Deserialize the response content into the entity</param>
        /// <returns>Returns the repository holder.</returns>
        protected virtual async Task<RepositoryHolder<KT, ET>> CallClient<KT, ET>(
              HttpMethod method, Uri uri
            , RepositorySettings options
            , HttpContent content = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpResponseMessage, RepositoryHolder<KT, ET>> mapOut = null
            , Action<HttpResponseMessage, byte[], RepositoryHolder<KT, ET>> deserializer = null)
        {
            var response = new RepositoryHolder<KT, ET>();

            try
            {
                //Create the message
                HttpRequestMessage httpRq = Request(method, uri);
                //Set the headers
                RequestHeadersSet(httpRq);
                //Sets the supported transport mechanisms
                RequestHeadersSetTransport(httpRq);
                //Sets the prefer headers
                RequestHeadersPreferSet(httpRq, options?.Prefer);
                //Sets the authentication.
                RequestHeadersAuth(httpRq);
                //Any manual adjustments.
                adjustIn?.Invoke(httpRq);

                //Sets the binary content to the request.
                if (content != null)
                    httpRq.Content = content;

                //Executes the request to the remote header.
                var httpRs = await Context.Client.SendAsync(httpRq);

                //Processes any response headers.
                ResponseHeadersAuth(httpRq, httpRs);

                //OK, set the response content if set
                if (httpRs.Content != null && httpRs.Content.Headers.ContentLength > 0)
                {
                    byte[] httpRsContent = await httpRs.Content.ReadAsByteArrayAsync();

                    if (httpRs.IsSuccessStatusCode)
                        deserializer?.Invoke(httpRs, httpRsContent, response);
                    else
                        // So that we can see error messages such as schema validation fail
                        response.ResponseMessage = Encoding.UTF8.GetString(httpRsContent);
                }

                //Get any outgoing trace headers and set them in to the response.
                //IEnumerable<string> trace;
                //if (httpRs.Headers.TryGetValues(ApimConstants.AzureTraceHeaderLocation, out trace))
                //    response.Settings.Prefer.Add(ApimConstants.AzureTraceHeaderLocation, trace.First());

                //Set the HTTP Response code.
                response.ResponseCode = (int)httpRs.StatusCode;

                //Maps any additional properties to the response.
                mapOut?.Invoke(httpRs, response);
            }
            catch (Exception ex)
            {
                response.ResponseMessage = FormatExceptionChain(ex);
                response.ResponseCode = 503;
            }

            return response;
        }
        #endregion

        #region EntitySerialize<ET>(ET entity)
        /// <summary>
        /// This method turns the entity in to binary content using
        /// the primary transport
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>The ByteArrayContent to transmit.</returns>
        protected virtual ByteArrayContent EntitySerialize<ET>(ET entity)
        {
            if (Equals(entity, default(ET)))
                throw new ArgumentNullException("entity");

            var data = Context.TransportSerializerDefault.GetData(entity);
            var content = new ByteArrayContent(data);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(Context.TransportOutDefault);

            return content;
        }
        #endregion
        #region EntityDeserialize<ET>(HttpResponseMessage rs, byte[] data)
        /// <summary>
        /// This method resolves the appropriate transport serializer from the incoming accept header.
        /// </summary>
        /// <param name="rs">The response</param>
        /// <param name="data">The response content</param>
        /// <returns>Returns true if the serializer can be resolved.</returns>
        protected virtual ET EntityDeserialize<ET>(HttpResponseMessage rs, byte[] data)
        {
            string mediaType = rs.Content.Headers.ContentType.MediaType;

            if (Context.TransportSerializers.ContainsKey(mediaType.ToLowerInvariant()))
            {
                var transport = Context.TransportSerializers[mediaType];
                return transport.GetObject<ET>(data);
            }

            throw new TransportSerializerResolutionException(mediaType);
        }
        #endregion


        /// <summary>
        /// This is the generic repository type, i.e. IRepositoryAsyncServer
        /// </summary>
        public Type RepositoryServerType { get; } = typeof(IRepositoryAsync<K, E>);
        /// <summary>
        /// This is the generic repository type, i.e. IRepository K,E
        /// </summary>
        public Type RepositoryType { get; } = typeof(IRepositoryAsync<K, E>);
        /// <summary>
        /// This is the entity type.
        /// </summary>
        public Type TypeEntity { get; } = typeof(E);
        /// <summary>
        /// This is the key type,
        /// </summary>
        public Type TypeKey { get; } = typeof(K);
    }
}
