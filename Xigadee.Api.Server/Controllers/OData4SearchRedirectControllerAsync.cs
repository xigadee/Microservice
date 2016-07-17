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

        #region Read -> Get(ApiRequest rq)
        /// <summary>
        /// Get=Read
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns a HTTP Action result</returns>
        //[Route("/")]
        [HttpGet]//http://host/service/
        //public virtual async Task<IHttpActionResult> Service([FromODataUri] string key, ODataQueryOptions queryOptions)
        public virtual async Task<IHttpActionResult> Service(ODataQueryOptions<object> request)
        {

    //        var valid = new System.Web.OData.Query.Validators.ODataQueryValidator();
    //        valid.Validate(
    //        Uri serviceRoot = new Uri("http://services.odata.org/V4/OData/OData.svc");
    //        IEdmModel model = EdmxReader.Parse(XmlReader.Create(serviceRoot + "/$metadata"));
    //        Uri fullUri = new Uri("http://services.odata.org/V4/OData/OData.svc/Products");

    ////        Uri fullUri = new Uri("Products?$select=ID&$expand=ProductDetail" +
    ////"&$filter=Categories/any(d:d/ID%20gt%201)&$orderby=ID%20desc" +
    ////"&$top=1&$count=true&$search=tom", UriKind.Relative);
    //        ODataUriParser parser = new ODataUriParser(model, fullUri);
    //        SelectExpandClause expand =
    //            parser.ParseSelectAndExpand();              //parse $select, $expand
    //        FilterClause filter = parser.ParseFilter();     // parse $filter
    //        OrderByClause orderby = parser.ParseOrderBy();  // parse $orderby
    //        SearchClause search = parser.ParseSearch();     // parse $search
    //        long? top = parser.ParseTop();                  // parse $top
    //        long? skip = parser.ParseSkip();                // parse $skip
    //        bool? count = parser.ParseCount();              // parse $count

            return StatusCode(HttpStatusCode.NotImplemented);
        }
        #endregion

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
