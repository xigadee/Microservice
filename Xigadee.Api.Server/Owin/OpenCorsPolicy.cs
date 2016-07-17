using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace Xigadee
{
    /// <summary>
    /// This policy accepts requests from any source.
    /// </summary>
    public class OpenCorrsPolicy: ICorsPolicyProvider
    {
        public async Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return new CorsPolicy() { AllowAnyHeader = true, AllowAnyMethod = true, AllowAnyOrigin = true };
        }
    }
}
