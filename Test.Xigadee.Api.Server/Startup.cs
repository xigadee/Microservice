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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Swashbuckle.Application;
using Unity.WebApi;
using Xigadee;

[assembly: OwinStartup(typeof(Test.Xigadee.Api.Server.Startup))]

namespace Test.Xigadee.Api.Server
{
    /// <summary>
    /// This is the standard startup class for the service.
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                var Service = new PopulatorWebApi();

                RouteConfig.Register(Service);

                SwaggerConfig.Register(Service);

                Service.Start(app, AzureHelper.Resolver);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     }

    /// <summary>
    /// This class is used to change the configuration to move the persistence commands to be registered
    /// locally within the Api service.
    /// </summary>
    public class StartupLocal
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                var Service = new PopulatorWebApi(true);

                RouteConfig.Register(Service);

                SwaggerConfig.Register(Service);

                Service.Start(app, AzureHelper.Resolver);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
