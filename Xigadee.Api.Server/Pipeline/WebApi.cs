using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Xigadee
{
    public static class WebApiExtensionMethods
    {
        public static WebApiMicroservicePipeline UpgradeToWebApiPipeline(this MicroservicePipeline pipeline, HttpConfiguration config = null)
        {
            return new WebApiMicroservicePipeline(pipeline, config);
        }

        public static WebApiMicroservicePipeline ToWebApiPipeline(this MicroservicePipeline pipeline)
        {
            var webpipe = pipeline as WebApiMicroservicePipeline;

            if (webpipe == null)
                throw new ArgumentOutOfRangeException("The incoming pipeline is not a WebApiMicroservicePipeline");

            return webpipe;
        }

        public static WebApiMicroservicePipeline AddMicroserviceUnavailableFilter(this WebApiMicroservicePipeline webpipe)
        {
            var filter = new WebApiServiceUnavailableFilter();

            webpipe.ApiConfig.Filters.Add(filter);
            webpipe.Service.StatusChanged += (object sender, StatusChangedEventArgs e) => filter.StatusCurrent = e.StatusNew;

            return webpipe;
        }

        public static WebApiMicroservicePipeline AddCorrelationIdFilter(this WebApiMicroservicePipeline webpipe)
        {
            var filter = new WebApiCorrelationIdFilter();

            webpipe.ApiConfig.Filters.Add(filter);

            return webpipe;
        }

        public static WebApiMicroservicePipeline AddVersionHeaderFilter(this WebApiMicroservicePipeline webpipe, string headerName = "X-XigadeeApiVersion")
        {
            var filter = new WebApiVersionHeaderFilter(headerName);

            webpipe.ApiConfig.Filters.Add(filter);

            return webpipe;
        }



    }


}
