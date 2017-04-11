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
using Microsoft.Owin;
using Owin;
using Xigadee;

[assembly: OwinStartup(typeof(Test.Xigadee.Api.Server.Startup))]

namespace Test.Xigadee.Api.Server
{
    public class StartupPipelineUnity
    {
        private void Hello1(UnityWebApiMicroservicePipeline pipe)
        {

        }

        public void Configuration(IAppBuilder app)
        {
            var pipeline = new UnityWebApiMicroservicePipeline();

            pipeline.CallOut(Hello1);
        }
    }

    /// <summary>
    /// This is the standard startup class for the service.
    /// </summary>
    public class StartupPipeline
    {
        private void Hello1(WebApiMicroservicePipeline pipe)
        {

        }

        private void IsTrue(WebApiMicroservicePipeline pipe)
        {

        }
        private void IsFalse(WebApiMicroservicePipeline pipe)
        {

        }

        private void Hello2(IPipelineChannelIncoming<WebApiMicroservicePipeline> pipeEx)
        {

        }

        private void IsTrueEx(IPipelineChannelIncoming<WebApiMicroservicePipeline> pipeEx)
        {

        }

        private void IsFalseEx(IPipelineChannelIncoming<WebApiMicroservicePipeline> pipeEx)
        {

        }

        public void Configuration(IAppBuilder app)
        {
            try
            {
                var pipe = new WebApiMicroservicePipeline();

                pipe
                    .CallOut(Hello1)
                    .Condition((c) => c.AuthIssuer() == null, IsTrue, IsFalse)
                    .AddChannelIncoming("mom")
                        .CallOut(Hello2)
                        .Condition((c) => c.TcpTlsConnection() == null, IsTrueEx, IsFalseEx)
                        
                    ;
                

                //RouteConfig.Register(Service);

                //SwaggerConfig.Register(Service);

                //Service.Start(app, AzureHelper.Resolver);

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
    public class StartupPipelineLocal
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                var Service = new WebApiMicroservicePipeline();

                RouteConfig.Register(Service);

                SwaggerConfig.Register(Service);

                Service.Start();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
