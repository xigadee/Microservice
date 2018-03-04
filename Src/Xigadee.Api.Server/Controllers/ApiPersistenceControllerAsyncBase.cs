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
using System.Xml.Schema;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.ModelBinding;
using System.Collections.Generic;
using System.Web.OData;
using System.Web.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class that handle standard CRUD based interaction between the Web and the provider.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class ApiPersistenceControllerAsyncBase<K, E> : ApiController
        where K : IEquatable<K>
    {
        #region Declarations

        private const string WildcardMediaType = @"*/*";

        /// <summary>
        /// This method is used to 
        /// </summary>
        protected IKeyMapper<K> mKeyMapper;
        /// <summary>
        /// This is the internal repository.
        /// </summary>
        protected IRepositoryAsync<K, E> mRespository;
        /// <summary>
        /// This is the supported transport serializers.
        /// </summary>
        protected Dictionary<string, TransportSerializer<E>> mSerializers;
        /// <summary>
        /// This property holds the OData query validation.
        /// </summary>
        protected readonly ODataValidationSettings mODataValidate;
        #endregion
        #region Constructor
        /// <summary>
        /// This constructors sets the underlying respository.
        /// </summary>
        /// <param name="respository">The aynsc repository.</param>
        protected ApiPersistenceControllerAsyncBase(IRepositoryAsync<K, E> respository)
        {
            mRespository = respository;
            mSerializers = TransportSerializer.GetSerializers<E>(GetType())
                .ToDictionary((p)=>p.MediaType.ToLowerInvariant());

            //If we have no serializers set on this controller, check the entity.
            if (mSerializers == null || mSerializers.Count == 0)
                mSerializers = TransportSerializer.GetSerializers<E>(typeof(E))
                    .ToDictionary((p) => p.MediaType.ToLowerInvariant());

            mKeyMapper = (KeyMapper<K>)KeyMapper.Resolve<K>();

            mODataValidate = ODataValidationSettingsCreate();
        }
        #endregion

        #region ODataValidationSettingsCreate()
        /// <summary>
        /// This method sets the OData validation options for incoming request.
        /// </summary>
        /// <returns>Returns the default settings that limit requests to simple quesries.</returns>
        protected virtual ODataValidationSettings ODataValidationSettingsCreate()
        {
            var settings = new ODataValidationSettings()
            {
                // Initialize settings as needed.
                AllowedFunctions = AllowedFunctions.None
                , AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.NotEqual | AllowedLogicalOperators.GreaterThan | AllowedLogicalOperators.LessThan
                , AllowedArithmeticOperators = AllowedArithmeticOperators.None
            };

            return settings;
        } 
        #endregion

        #region ResolveSerializer(ApiRequest rq, out TransportSerializer<E> transport)
        /// <summary>
        /// This method resolves the transport serializer for the outgoing response using the accept header.
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <param name="transport">The resolve transport serializer.</param>
        /// <returns>Returns true if the serializer can be resolved.</returns>
        protected virtual bool ResolveSerializer(ApiRequest rq, out TransportSerializer<E> transport)
        {
            transport = null;
            var accepts = rq.Accept ?? new List<MediaTypeWithQualityHeaderValue>();
            if (accepts.Count == 0)
                accepts.Add(new MediaTypeWithQualityHeaderValue(WildcardMediaType));

            if (mSerializers.Count == 0)
                return false;

            try
            {
                foreach (var accept in accepts.OrderByDescending(a => a.Quality))
                {
                    var mediaType = accept.MediaType.ToLowerInvariant();
                    if (mSerializers.ContainsKey(mediaType))
                    {
                        transport = mSerializers[mediaType];
                        return true;
                    }
                    if (mediaType.Equals(WildcardMediaType) || mediaType.EndsWith("*"))
                    {
                        transport = mSerializers.OrderByDescending(s => s.Value.Priority).First().Value;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(rq, ex);
            }

            return false;
        }
        #endregion
        #region ResolveDeserializer(ApiRequest rq, out TransportSerializer<E> transport)
        /// <summary>
        /// This method resolves the appropriate transport serialzer from the incoming accept header.
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <param name="transport">The resolve transport serializer.</param>
        /// <returns>Returns true if the serializer can be resolved.</returns>
        protected virtual bool ResolveDeserializer(ApiRequest rq, out TransportSerializer<E> transport)
        {
            transport = null;

            try
            {
                string mediaType = rq.BodyType.MediaType.ToLowerInvariant();
                if (mSerializers.ContainsKey(mediaType))
                {
                    transport = mSerializers[mediaType];
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogException(rq, ex);
            }

            return false;
        }
        #endregion

        #region LogException(ApiRequest rq, Exception ex)
        /// <summary>
        /// This method logs any processing exceptions.
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <param name="ex">The exception.</param>
        protected virtual void LogException(ApiRequest rq, Exception ex) { }
        #endregion

        #region LogMessage
        /// <summary>
        /// This method logs any processing messages.
        /// </summary>
        /// <param name="loggingLevel">Logging level of the message</param>
        /// <param name="message">The log message</param>
        protected virtual void LogMessage(LoggingLevel loggingLevel, string message) { }
        #endregion

        #region EntityValidate(ApiRequest rq, E entity)
        /// <summary>
        /// This method can be used to validate the incoming entity before it is passed
        /// to the create or update methods.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool EntityValidate(ApiRequest rq, E entity)
        {
            return true;
        } 
        #endregion

        #region Create -> Post(ApiRequest rq)
        /// <summary>
        /// Post=Create
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        [HttpPost]
        public virtual async Task<IHttpActionResult> Post([ModelBinder(typeof(ApiRequestModelBinder))]ApiRequest rq)
        {
            TransportSerializer<E> entitySerializer, entityDeserializer;
            //Check that we have an appropriate deserializer
            if (!ResolveDeserializer(rq, out entityDeserializer))
                return StatusCode(HttpStatusCode.UnsupportedMediaType);

            //Check that we have an appropriate serializer in the accept header
            if (!ResolveSerializer(rq, out entitySerializer))
                return StatusCode(HttpStatusCode.NotAcceptable);

            try
            {
                E entity = entityDeserializer.GetObject(rq.Body);

                //Allow for validate before submission to the upstream server.
                if (!EntityValidate(rq, entity))
                    return StatusCode(HttpStatusCode.BadRequest);

                var response = await mRespository.Create(entity, rq.Options);

                return ResponseFormat(response, entitySerializer);
            }
            catch (XmlSchemaValidationException ex)
            {
                LogException(rq, ex);
                return BadRequest(ex.Message);
            }
            catch (TransportDeserializationException tdex)
            {
                LogException(rq, tdex);
                return StatusCode(HttpStatusCode.BadRequest);
            }
            catch (TransportSerializationException tsex)
            {
                LogException(rq, tsex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                LogException(rq, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }     
        }
        #endregion
        #region Read -> Get(ApiRequest rq)
        /// <summary>
        /// Get=Read
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        [HttpGet]
        public virtual async Task<IHttpActionResult> Get([ModelBinder(typeof(ApiRequestModelBinder))]ApiRequest rq)
        {
            TransportSerializer<E> entitySerializer;
            //Check that we have an appropriate serializer in the accept header
            if (!ResolveSerializer(rq, out entitySerializer))
                return StatusCode(HttpStatusCode.NotAcceptable);

            try
            {
                RepositoryHolder<K, E> response;
                if (rq.HasKey)
                    response = await mRespository.Read(mKeyMapper.ToKey(rq.Id), rq.Options);
                else if (rq.HasReference)
                    response = await mRespository.ReadByRef(rq.RefType, rq.RefValue, rq.Options);
                else
                    return BadRequest();

                return ResponseFormat(response, entitySerializer);
            }
            catch (Exception ex)
            {
                LogException(rq, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #region Update -> Put(ApiRequest rq)
        /// <summary>
        /// Put=Update
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        [HttpPut]
        public virtual async Task<IHttpActionResult> Put([ModelBinder(typeof(ApiRequestModelBinder))]ApiRequest rq)
        {
            TransportSerializer<E> entitySerializer, entityDeserializer;
            //Check that we have an appropriate deserializer
            if (!ResolveDeserializer(rq, out entityDeserializer))
                return StatusCode(HttpStatusCode.UnsupportedMediaType);

            //Check that we have an appropriate serializer in the accept header
            if (!ResolveSerializer(rq, out entitySerializer))
                return StatusCode(HttpStatusCode.NotAcceptable);

            try
            {
                E entity = entityDeserializer.GetObject(rq.Body);

                //Allow for validate before submission to the upstream server.
                if (!EntityValidate(rq, entity))
                    return StatusCode(HttpStatusCode.BadRequest);

                var response = await mRespository.Update(entity, rq.Options);

                return ResponseFormat(response, entitySerializer);
            }
            catch (XmlSchemaValidationException ex)
            {
                LogException(rq, ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                LogException(rq, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #region Delete -> Delete(ApiRequest rq)
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        [HttpDelete]
        public virtual async Task<IHttpActionResult> Delete([ModelBinder(typeof(ApiRequestModelBinder))]ApiRequest rq)
        {
            try
            {
                RepositoryHolder<K, Tuple<K, string>> response;
                if (rq.HasKey)
                    response = await mRespository.Delete(mKeyMapper.ToKey(rq.Id), rq.Options);
                else if (rq.HasReference)
                    response = await mRespository.DeleteByRef(rq.RefType, rq.RefValue, rq.Options);
                else
                    return BadRequest();

                return ResponseFormat(response);
            }
            catch (Exception ex)
            {
                LogException(rq, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #region Version -> Head(ApiRequest rq)
        /// <summary>
        /// Head=Version
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        [HttpHead]
        public virtual async Task<IHttpActionResult> Head([ModelBinder(typeof(ApiRequestModelBinder))]ApiRequest rq)
        {
            try
            {
                RepositoryHolder<K, Tuple<K, string>> response;
                if (rq.HasKey)
                    response = await mRespository.Version(mKeyMapper.ToKey(rq.Id), rq.Options);
                else if (rq.HasReference)
                    response = await mRespository.VersionByRef(rq.RefType, rq.RefValue, rq.Options);
                else
                    return BadRequest();

                return ResponseFormat(response);
            }
            catch (Exception ex)
            {
                LogException(rq, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Search(ODataQueryOptions<E> request)

        /// <summary>
        /// Search=Search
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        [AcceptVerbs("Search")]
        public virtual async Task<IHttpActionResult> Search(ODataQueryOptions<E> request)
        {
            try
            {
                //Validate that the request is correctly formed.
                request.Validate(mODataValidate);

                //Load the request with the necessary parameters.
                SearchRequest rq = new SearchRequest();
                rq.ODataPopulate(request);

                //Make the request to the persistence service.
                RepositorySettings settings = ApiUtility.BuildRepositorySettings(ActionContext);
                RepositoryHolder<SearchRequest, SearchResponse> response = await mRespository.Search(rq, settings);

                return new OData4ServiceDocumentResponse(response, ActionContext.Request.RequestUri);
            }
            catch (Exception vex)
            {
                return BadRequest($"Unable to process Odata request - {vex.Message}");
            }
        }
        #endregion

        #region ResponseFormat<KT, ET> ...
        /// <summary>
        /// This method processes the outgoing response.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="responseHolder">The response message.</param>
        /// <param name="entitySerializer">The optional serializer.</param>
        /// <returns>Returns the relevant result.</returns>
        protected virtual IHttpActionResult ResponseFormat<KT, ET>(RepositoryHolder<KT, ET> responseHolder
            , TransportSerializer<ET> entitySerializer = null)
        {
            ApiResponse response = null;

            switch (responseHolder.ResponseCode)
            {
                case 200:
                    response = new ApiResponse(HttpStatusCode.OK);
                    break;
                case 201:
                    response = new ApiResponse(HttpStatusCode.Created);
                    break;
                case 202:
                    response = new ApiResponse(HttpStatusCode.Accepted);
                    break;
                case 204:
                    response = new ApiResponse(HttpStatusCode.NoContent);
                    break;
                case 400:
                    return BadRequest(responseHolder.ResponseMessage ?? string.Empty);                                
                case 404:
                    return StatusCode(HttpStatusCode.NotFound);
                case 405:
                    return StatusCode(HttpStatusCode.MethodNotAllowed);
                case 408:
                    return StatusCode(HttpStatusCode.RequestTimeout);
                case 409:
                    return StatusCode(HttpStatusCode.Conflict);
                case 412:
                    return StatusCode(HttpStatusCode.PreconditionFailed);
                case 417:
                    return StatusCode(HttpStatusCode.ExpectationFailed);
                case 442:
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                case 502:
                    return StatusCode(HttpStatusCode.BadGateway);
                case 504:
                    return StatusCode(HttpStatusCode.GatewayTimeout);
                default:
                    LogMessage(LoggingLevel.Error, $"Unexpected error occurred {responseHolder}");
                    return StatusCode(HttpStatusCode.InternalServerError);
            }

            if (entitySerializer != null && responseHolder.Entity != null)
            {
                response.MediaType = entitySerializer.MediaType;
                response.Data = entitySerializer.GetData(responseHolder.Entity);
            }

            if (responseHolder.KeyReference != null)
            {
                response.ContentId = responseHolder.KeyReference.Item1;
                response.VersionId = responseHolder.KeyReference.Item2;
            }

            if (responseHolder.IsCached)
                response.IsCached = true;

            return response;
        }
        #endregion
    }
}
