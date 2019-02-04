//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;

//namespace Xigadee
//{
//    //https://blogs.msdn.microsoft.com/kaevans/2016/04/13/azure-web-app-client-certificate-authentication-with-asp-net-core-2/
//    //https://docs.microsoft.com/en-us/azure/app-service/app-service-web-configure-tls-mutual-auth
//    public class ClientCertificationAuthenticationFilter
//    {

//    }

//    public class ClientCertificationAuthenticationMiddleware
//    {
//        private readonly RequestDelegate mNext;

//        public ClientCertificationAuthenticationMiddleware(RequestDelegate next)
//        {
//            mNext = next;
//        }

//        public async Task Invoke(HttpContext httpContext)
//        {
//            var cert = httpContext.Connection.ClientCertificate;

//            await mNext(httpContext);
//        }
//    }

//    public class ClientCertificateAuthorizationFilter: IAsyncAuthorizationFilter
//    {
//        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
//        {
//            var cert = context.HttpContext.Connection.ClientCertificate;

//            //if (cert == null)
//            //{
//            //    // If no certificate has been supplied but this controller/action allows no certificate
//            //    // access then let this call through
//            //    if (context.ActionDescriptor.GetCustomAttributes<AllowNoClientCertificateAttribute>().Any() ||
//            //        actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowNoClientCertificateAttribute>().Any())
//            //        return continuation();

//            //    HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Unauthorized);
//            //    response.Content = new StringContent("Client certificate missing");
//            //    return Task.FromResult(response);
//            //}

//            return Task.CompletedTask;
//        }
//    }
//}
