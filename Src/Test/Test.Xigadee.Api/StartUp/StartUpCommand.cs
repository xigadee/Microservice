using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Xigadee;

namespace Test.Xigadee.Api
{
    public class MyStuff
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class StartUpCommand
    {
        private byte[] mSecret = Encoding.ASCII.GetBytes(JwtTests.SecretPass);

        public void Configuration(IAppBuilder app)
        {
            var webpipe = new UnityWebApiMicroservicePipeline();
            PersistenceClient<Guid, MyStuff> init;

            webpipe
                .ApiConfig((c) => c.Routes.MapHttpRoute("Default", "api/{controller}/{id}", new { id = RouteParameter.Optional }))
                .ApiConfig((c) => c.Services.Replace(typeof(IHttpControllerSelector), new BypassCacheSelector(c)))
                .ApiAddMicroserviceUnavailableFilter()
                .ApiAddJwtTokenAuthentication(JwtHashAlgorithm.HS256, mSecret, audience: JwtTests.Audience)
                .AddChannelIncoming("incoming", internalOnly: true)
                    .AttachCommand(new PersistenceManagerHandlerMemory<Guid, MyStuff>((e) => e.Id, (s) => new Guid(s)))
                    .Revert()
                .AddChannelOutgoing("outgoing", internalOnly: true)
                    .AttachPersistenceMessageInitiatorUnity(out init, "incoming")
                    .Revert()
                //.AttachCommand(new Per
                ;

            webpipe.StartWebApi(app);

        }
    }

    public class BypassCacheSelector: DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        public BypassCacheSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var assembly = Assembly.LoadFile("c:/myAssembly.dll");
            var types = assembly.GetTypes(); //GetExportedTypes doesn't work with dynamic assemblies
            var matchedTypes = types.Where(i => typeof(IHttpController).IsAssignableFrom(i)).ToList();

            var controllerName = base.GetControllerName(request);
            var matchedController =
                matchedTypes.FirstOrDefault(i => i.Name.ToLower() == controllerName.ToLower() + "controller");

            return new HttpControllerDescriptor(_configuration, controllerName, matchedController);
        }
    }
}
