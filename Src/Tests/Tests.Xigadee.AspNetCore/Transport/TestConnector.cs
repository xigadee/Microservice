using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee
{
    public class TestConnector : ApiProviderBase
    {
        public TestConnector() 
            : base(new Uri("http://localhost"))
        {
        }

        /// <summary>
        /// This is the full registration method.
        /// </summary>
        /// <param name="info">The registration info.</param>
        /// <returns></returns>
        public async Task<ApiResponse> Register(RegistrationModel info) =>
            (await CallApiClient(HttpMethod.Post, UriBaseAppend("api/registration/register"), info ?? throw new ArgumentNullException(nameof(info))));

    }
}
