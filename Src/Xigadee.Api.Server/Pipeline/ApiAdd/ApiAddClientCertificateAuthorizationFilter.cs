using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Xigadee
{
    public static partial class WebApiExtensionMethods
    {
        /// <summary>
        /// Adds a client certificate authorization filter to the pipeline - validates that client certificates are passed 
        /// </summary>
        /// <typeparam name="P">IPipelineWebApi Type</typeparam>
        /// <param name="webpipe">Web Api Pipeline</param>
        /// <param name="verifyCertificate">Indicates the client certificate should be verified to be a valid certificate</param>
        /// <param name="clientCertificateThumbprints">A list of trusted certificate thumbprints</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static P ApiAddClientCertificateAuthorization<P>(this P webpipe, bool verifyCertificate, List<string> clientCertificateThumbprints, Action<IAuthorizationFilter> action = null) where P : IPipelineWebApi
        {
            var filter = new ClientCertificateAuthorizationFilter(verifyCertificate, clientCertificateThumbprints);
            action?.Invoke(filter);
            webpipe.HttpConfig.Filters.Add(filter);
            return webpipe;
        }
    }
}
