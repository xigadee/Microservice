using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Web.Http.OData.Routing;
using Xigadee;
using Microsoft.Data.OData;

namespace Test.Xigadee.Api.Server.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Xigadee;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<ApiRequest>("ApiRequests");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ApiRequestsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        // GET: odata/ApiRequests
        public async Task<IHttpActionResult> GetApiRequests(ODataQueryOptions<ApiRequest> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            // return Ok<IEnumerable<ApiRequest>>(apiRequests);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // GET: odata/ApiRequests(5)
        public async Task<IHttpActionResult> GetApiRequest([FromODataUri] string key, ODataQueryOptions<ApiRequest> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            // return Ok<ApiRequest>(apiRequest);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // PUT: odata/ApiRequests(5)
        public async Task<IHttpActionResult> Put([FromODataUri] string key, Delta<ApiRequest> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Put(apiRequest);

            // TODO: Save the patched entity.

            // return Updated(apiRequest);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // POST: odata/ApiRequests
        public async Task<IHttpActionResult> Post(ApiRequest apiRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Add create logic here.

            // return Created(apiRequest);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // PATCH: odata/ApiRequests(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] string key, Delta<ApiRequest> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Patch(apiRequest);

            // TODO: Save the patched entity.

            // return Updated(apiRequest);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // DELETE: odata/ApiRequests(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] string key)
        {
            // TODO: Add delete logic here.

            // return StatusCode(HttpStatusCode.NoContent);
            return StatusCode(HttpStatusCode.NotImplemented);
        }
    }
}
