using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Xigadee
{
    public class ClientCertificateAuthorizationFilter : IAuthorizationFilter
    {
        private readonly bool mVerifyCertificate;
        private readonly List<string> mClientCertificateThumbprints;
        private readonly HashSet<string> mValidThumbprints; 

        public bool AllowMultiple => false;

        public ClientCertificateAuthorizationFilter(bool verifyCertificate, List<string> clientCertificateThumbprints)
        {
            mVerifyCertificate = verifyCertificate;
            mClientCertificateThumbprints = clientCertificateThumbprints;
            mValidThumbprints = new HashSet<string>();
        }

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            HttpContext context = HttpContext.Current;
            var request = actionContext.Request;

            if (!context.Request.ClientCertificate.IsPresent)
            {
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("Client certificate missing");
                return Task.FromResult(response);
            }

            try
            {
                var clientCert = new X509Certificate2(context.Request.ClientCertificate.Certificate);
                if (DateTime.Now < clientCert.NotBefore || DateTime.Now > clientCert.NotAfter)
                {
                    HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent($"Client certificate has expired or not yet valid {clientCert.NotBefore}-{clientCert.NotAfter}");
                    return Task.FromResult(response);
                }

                if (mVerifyCertificate && !clientCert.Verify())
                {
                    HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent("Client certificate not valid");
                    return Task.FromResult(response);
                }

                // If we have already validated this thumbprint then continue
                if (!string.IsNullOrEmpty(clientCert.Thumbprint) && mValidThumbprints.Contains(clientCert.Thumbprint))
                    return continuation();

                // If we have thumbprints to verify against check the certificate is in the list
                if (mClientCertificateThumbprints.Any() &&
                    mClientCertificateThumbprints.FirstOrDefault(m => m.Equals(clientCert.Thumbprint?.Trim(), StringComparison.InvariantCultureIgnoreCase)) == null)
                {
                    HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Unauthorized);
                    response.Content = new StringContent($"Client thumbprint does not match an expected value {clientCert.Thumbprint?.Trim()}");
                    return Task.FromResult(response);
                }

                if (!string.IsNullOrEmpty(clientCert.Thumbprint) && !mValidThumbprints.Contains(clientCert.Thumbprint))
                    mValidThumbprints.Add(clientCert.Thumbprint);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent($"Cannot check client certificate - {ex.Message}");
                return Task.FromResult(response);
            }

            // All good so continue
            return continuation();
        }
    }
}