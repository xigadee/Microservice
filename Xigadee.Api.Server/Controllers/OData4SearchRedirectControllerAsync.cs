#region using

using System.Xml.Schema;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Collections.Generic;
using System.Web.OData;
using System.Web.OData.Query;
using Microsoft.Data.OData.Query;

#endregion
namespace Xigadee
{
    public abstract class OData4SearchRedirectControllerAsync: ApiController
    {
        #region Metadata()
        /// <summary>
        /// Get=Read
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        //[Route("/$metadata")]// http://host/service/$metadata
        [HttpGet]
        public virtual async Task<IHttpActionResult> Metadata()
        {
            //TransportSerializer<E> entitySerializer;
            ////Check that we have an appropriate serializer in the accept header
            //if (!ResolveSerializer(rq, out entitySerializer))
            //    return StatusCode(HttpStatusCode.NotAcceptable);

            //try
            //{
            //    RepositoryHolder<K, E> response;
            //    if (rq.HasKey)
            //        response = await mRespository.Read(mKeyMapper.ToKey(rq.Id), rq.Options);
            //    else if (rq.HasReference)
            //        response = await mRespository.ReadByRef(rq.RefType, rq.RefValue, rq.Options);
            //    else
            //        return BadRequest();

            //    return ResponseFormat(response, entitySerializer);
            //}
            //catch (Exception ex)
            //{
            //    LogException(rq, ex);
            return StatusCode(HttpStatusCode.NotImplemented);
            //}
        }
        #endregion
        #region Batch()
        /// <summary>
        /// Get=Read
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        //[Route("/$metadata")]// http://host/service/$batch
        [HttpGet]
        public virtual async Task<IHttpActionResult> Batch()
        {
            return StatusCode(HttpStatusCode.NotImplemented);
        }
        #endregion

    }
}
