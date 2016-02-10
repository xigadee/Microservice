#region using
using System.Net.Http;
using System.Text;
using System.Web.Http.Results;
using System.Xml.Linq;

using System;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO; 
#endregion
namespace Xigadee.Api
{
    /// <summary>
    /// This is the abstract base class that handle standard CRUD based interaction between the Web and the provider.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">The serialization type.</typeparam>
    public abstract class ApiPersistenceControllerBase<K, E, S> : ApiController
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This is the internal repository.
        /// </summary>
        protected IRepository<K, E> mRespository;
        #endregion

        MediaTypeHeaderValue mHeader;
        MediaTypeFormatter mFormatter;

        protected ApiPersistenceControllerBase(IRepository<K, E> respository)
        {
            mRespository = respository;
        }

        protected abstract K KeyMaker(string key);

        protected abstract S OutputMaker(E entity);

        protected abstract E EntityMaker(S data);

        protected abstract bool ValidateInputData(S data, out string error);

        protected virtual void LogException(string id, Exception ex)
        {

        }

        #region Post([FromUri] string id, [FromBody] byte[] data)
        /// <summary>
        /// Post=Create
        /// </summary>
        /// <param name="id">The id of the entity.</param>
        /// <param name="data">The entity body.</param>
        /// <returns>Response 201</returns>
        [Route("")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> Post([FromUri] string id, [FromBody] S data)
        {
            try
            {
                RepositoryOptions opts = new RepositoryOptions();
                string validationError;
                if (!ValidateInputData(data, out validationError))
                    return new BadRequestErrorMessageResult(validationError, this);

                var entity = EntityMaker(data);

                mRespository.Create(entity, ref opts);

                return ResponseTranslate(opts);
            }
            catch (Exception ex)
            {
                LogException(id, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #region Get([FromUri] string id)
        /// <summary>
        /// Get=Read
        /// </summary>
        /// <param name="id">The id of the entity.</param>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public virtual async Task<IHttpActionResult> Get([FromUri] string id)
        {
            try
            {
                RepositoryOptions opts = new RepositoryOptions();
                var key = KeyMaker(id);

                var entity = mRespository.Read(key, ref opts);

                return ResponseTranslate(opts, entity);
            }
            catch (Exception ex)
            {
                LogException(id, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #region Put([FromUri] string id, [FromBody] byte[] data)
        /// <summary>
        /// Put=Update
        /// </summary>
        /// <param name="id">The id of the entity.</param>
        /// <param name="data">The entity body.</param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public virtual async Task<IHttpActionResult> Put([FromUri] string id, [FromBody] S data)
        {
            try
            {
                RepositoryOptions opts = new RepositoryOptions();
                string validationError;
                if (!ValidateInputData(data, out validationError))
                    return new BadRequestErrorMessageResult(validationError, this);

                var entity = EntityMaker(data);

                mRespository.Update(entity, ref opts);
                return ResponseTranslate(opts);
            }
            catch (Exception ex)
            {
                LogException(id, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #region Delete([FromUri] string id)
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id">The id of the entity.</param>
        /// <returns></returns>
        [Route("")]
        [HttpDelete]
        public virtual async Task<IHttpActionResult> Delete([FromUri] string id)
        {
            try
            {
                RepositoryOptions opts = new RepositoryOptions();
                var key = KeyMaker(id);

                mRespository.Delete(key, ref opts);

                return ResponseTranslate(opts);
            }
            catch (Exception ex)
            {
                LogException(id, ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        private string SqlServerErrorTranslate(int errorCode)
        {
            switch (errorCode)
            {
                case 400: return "Item(s) not found. ";
                case 401: return "Attempt to delete/update an item that doesn't exist. ";
                case 404: return "Item not found. ";
                case 408: return "Account does not exist. ";
                case 409: return "Item conflicts with another existing User Reference. ";
                case 410: return "Cannot update a deleted item. ";
                case 412: return "Attempt to create an item that already exists, key must be unique.";
                case 413: return "Parent account does not exist. ";
                default: return "";
            }
        }

        protected virtual IHttpActionResult ResponseTranslate(RepositoryOptions opts, E entity)
        {
            switch (opts.ResponseCode)
            {
                case 200:
                case 201:
                    return Ok(OutputMaker(entity));
                default:
                    return ResponseTranslate(opts);
            }
        }

        protected virtual IHttpActionResult ResponseTranslate(RepositoryOptions opts)
        {
            var response = Request.CreateResponse(SqlServerErrorTranslate(opts.ResponseCode));

            switch (opts.ResponseCode)
            {
                case 200:
                    return Ok();
                case 400:
                    response.StatusCode = HttpStatusCode.BadRequest; return ResponseMessage(response);
                case 404:
                    response.StatusCode = HttpStatusCode.NotFound; return ResponseMessage(response);
                case 405:
                    response.StatusCode = HttpStatusCode.MethodNotAllowed; return ResponseMessage(response);
                case 408:
                    response.StatusCode = HttpStatusCode.RequestTimeout; return ResponseMessage(response);
                case 412:
                    response.StatusCode = HttpStatusCode.PreconditionFailed; return ResponseMessage(response);
                case 417:
                    response.StatusCode = HttpStatusCode.ExpectationFailed; return ResponseMessage(response);
                case 442:
                    response.StatusCode = HttpStatusCode.ExpectationFailed; return ResponseMessage(response);
                case 504:
                    response.StatusCode = HttpStatusCode.GatewayTimeout; return ResponseMessage(response);
                default:
                    response.StatusCode = HttpStatusCode.InternalServerError; return ResponseMessage(response);
            }
        }

        protected byte[] ToBytes(XElement xml)
        {
            return Encoding.UTF8.GetBytes(xml.ToString());
        }
    }
}
