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
        public static WebApiMicroservicePipeline UpgradeToWebApiPipeline(this MicroservicePipeline pipeline
            , HttpConfiguration httpConfig = null)
        {
            return new WebApiMicroservicePipeline(pipeline, httpConfig);
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

            webpipe.HttpConfig.Filters.Add(filter);
            webpipe.Service.StatusChanged += (object sender, StatusChangedEventArgs e) => filter.StatusCurrent = e.StatusNew;

            return webpipe;
        }

        public static WebApiMicroservicePipeline AddCorrelationIdFilter(this WebApiMicroservicePipeline webpipe)
        {
            var filter = new WebApiCorrelationIdFilter();

            webpipe.HttpConfig.Filters.Add(filter);

            return webpipe;
        }

        public static WebApiMicroservicePipeline AddVersionHeaderFilter(this WebApiMicroservicePipeline webpipe, string headerName = "X-XigadeeApiVersion")
        {
            var filter = new WebApiVersionHeaderFilter(headerName);

            webpipe.HttpConfig.Filters.Add(filter);

            return webpipe;
        }



    }


}
