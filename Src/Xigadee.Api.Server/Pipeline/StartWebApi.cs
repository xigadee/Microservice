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
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using Owin;

namespace Xigadee
{
    public static partial class WebApiExtensionMethods
    {
        /// <summary>
        /// This method can be used to start the web api pipeline using the 
        /// HttpConfiguration embedded in the pipeline.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="webpipe">The pipeline based on the WebApi.</param>
        /// <param name="app">The app builder reference.</param>
        /// <returns>Returns the pipeline</returns>
        public static void StartWebApi<P>(this P webpipe, IAppBuilder app)
            where P : IPipelineWebApi
        {
            app.UseWebApi(webpipe.HttpConfig);

            webpipe.HttpConfig.EnsureInitialized();

            webpipe.HttpConfig.Filters
                .Where((f) => f.Instance is IRequireMicroserviceConnection)
                .ForEach((f) => ((IRequireMicroserviceConnection)f.Instance).Microservice = webpipe.ToMicroservice());            

            Task.Run(() => webpipe.Start());
        }  
    }
}
