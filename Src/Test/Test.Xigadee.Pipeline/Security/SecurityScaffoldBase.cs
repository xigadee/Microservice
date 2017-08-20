using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This class provides the basic scaffolding to allow for simple security testing from a WebApi front-end
    /// to a backend Microservice.
    /// </summary>
    public abstract class SecurityScaffoldBase
    {
        protected UnityWebApiMicroservicePipeline mWebApi;
        protected MicroservicePipeline mService1;

        protected ICommunicationBridge mBridgeRequest;
        protected ICommunicationBridge mBridgeResponse;

        protected virtual void Init()
        {
            var fabric = new ManualFabricBridge();
            mBridgeRequest = new ManualCommunicationBridgeAgent(fabric, CommunicationBridgeMode.RoundRobin);
            mBridgeResponse = new ManualCommunicationBridgeAgent(fabric, CommunicationBridgeMode.Broadcast);

            mWebApi = new UnityWebApiMicroservicePipeline("Web")
                .CallOut(WebApiConfigure)
                .AddChannelOutgoing("Request", "This is the outgoing request channel")
                    .AttachSender(mBridgeRequest.GetSender())
                    .Revert()
                .AddChannelIncoming("Response", "This is the response channel back from the Service")
                    .AttachListener(mBridgeResponse.GetListener())
                    .Revert();               
                

            mService1 = new MicroservicePipeline("Service")
                .CallOut(ServiceConfigure)
                .AddChannelIncoming("Request", "This is the incoming request channel from the API")
                    .AttachListener(mBridgeRequest.GetListener())
                    .Revert()
                .AddChannelOutgoing("Response", "This is the outgoing request channel")
                    .AttachSender(mBridgeResponse.GetSender())
                    .Revert()
                    ;


            mService1.Start();
            mWebApi.Start();

        }

        protected abstract void WebApiConfigure(UnityWebApiMicroservicePipeline pipeline);

        protected abstract void ServiceConfigure(MicroservicePipeline pipeline);
    }

    public class MyResolver: IAssembliesResolver//DefaultAssembliesResolver
    {
        public ICollection<Assembly> GetAssemblies()
        {
            throw new NotImplementedException();
        }
    }

    public class MyClass2: IHttpControllerTypeResolver //DefaultHttpControllerTypeResolver, 
    {
        public ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
        {
            throw new NotImplementedException();
        }
    }

    public class MyClass: IHttpControllerSelector
    {
        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            throw new NotImplementedException();
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }
    }
}
