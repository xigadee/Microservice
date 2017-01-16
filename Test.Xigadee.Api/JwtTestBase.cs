using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Xigadee;

namespace Test.Xigadee.Api
{
    public abstract class JwtTestBase
    {
        protected TestServer mServer;

        #region Initialization
        public virtual void FixtureInit()
        {
            mServer = TestServer.Create<StartUp>();
        }

        public virtual void FixtureCleanUp()
        {
            mServer.Dispose();
        }
        #endregion

        /// <summary>
        /// This method allows the Microservice to restart.
        /// </summary>
        /// <param name="jwt">The jwt token header value.</param>
        /// <param name="message">The function to generate the message.</param>
        /// <returns>Returns the response message.</returns>
        public HttpResponseMessage ReadWithRetry(string jwt, Func<HttpRequestMessage> message, int maxRetry = 20)
        {
            HttpResponseMessage response;

            do
            {
                var rq = message();
                if (jwt != null)
                    rq.Headers.Authorization = new AuthenticationHeaderValue("bearer", jwt);

                response = mServer.HttpClient
                    .SendAsync(rq)
                    .Result;

                maxRetry--;

                if (maxRetry < 0)
                    throw new ArgumentOutOfRangeException("maxRetry", maxRetry, "maxRetry has been exceeded");
            }
            while (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable);

            return response;
        }
    }
}
