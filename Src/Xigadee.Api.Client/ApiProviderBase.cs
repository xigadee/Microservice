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

#region using
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This abstract base class is used for api client connectivity.
    /// </summary>
    public abstract class ApiProviderBase
    {
        #region Declarations
        /// <summary>
        /// This is the assembly version
        /// </summary>
        protected readonly string mAssemblyVersion;
        /// <summary>
        /// This is a list of auth handlers to be used to authorise the request.
        /// </summary>
        protected List<IApiProviderAuthBase> mAuthHandlers;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        protected ApiProviderBase(IEnumerable<IApiProviderAuthBase> authHandlers = null)
        {
            // Get the types assembly version to add to the request headers
            mAssemblyVersion = AssemblyVersionGet();
            mAuthHandlers = authHandlers?.ToList();
        }
        #endregion

        #region AssemblyVersionGet()
        /// <summary>
        /// This method returns the assembly version that is passed to the calling party. You can override this
        /// method to change the version.
        /// </summary>
        /// <returns>Returns a string containing the assembly version.</returns>
        protected virtual string AssemblyVersionGet()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
        #endregion

        #region Request(HttpMethod verb, Uri uri)
        /// <summary>
        /// This method creates the default request message.
        /// </summary>
        /// <param name="verb">The HTTP verb.</param>
        /// <param name="uri">The Uri request.</param>
        /// <returns>Returns the message with the full domain request.</returns>
        protected virtual HttpRequestMessage Request(HttpMethod verb, Uri uri)
        {
            HttpRequestMessage rq = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = uri
            };

            return rq;
        }
        #endregion
        #region RequestHeadersSet(HttpRequestMessage rq)
        /// <summary>
        /// This virtual method sets the necessary headers for the request.
        /// </summary>
        /// <param name="rq">The http request.</param>
        protected virtual void RequestHeadersSet(HttpRequestMessage rq)
        {
            rq.Headers.Add("x-api-clientversion", mAssemblyVersion);
            rq.Headers.Add("x-api-version", "2016-08-01");
        }
        #endregion
        #region RequestHeadersPreferSet(HttpRequestMessage rq, Dictionary<string, string> Prefer)
        /// <summary>
        /// This method sets the prefer request header directives for the Api call.
        /// </summary>
        /// <param name="rq">The http request object.</param>
        /// <param name="Prefer">The prefer collection.</param>
        protected virtual void RequestHeadersPreferSet(HttpRequestMessage rq, Dictionary<string, string> Prefer)
        {
            if (Prefer != null && Prefer.Count > 0)
                rq.Headers.Add("Prefer", Prefer.Select((k) => string.Format("{0}={1}", k.Key, k.Value)));
        }
        #endregion

        #region RequestHeadersAuthSet(HttpRequestMessage rq)
        /// <summary>
        /// This method sets the prefer request headers for the Api call.
        /// </summary>
        /// <param name="rq">The http request object.</param>
        /// <param name="Prefer">The prefer collection.</param>
        protected virtual void RequestHeadersAuth(HttpRequestMessage rq)
        {
        }
        #endregion
        #region ResponseHeadersAuth(HttpRequestMessage rq, HttpResponseMessage rs)
        /// <summary>
        /// This method sets the prefer request headers for the Api call.
        /// </summary>
        /// <param name="rq">The http request object.</param>
        /// <param name="Prefer">The prefer collection.</param>
        protected virtual void ResponseHeadersAuth(HttpRequestMessage rq, HttpResponseMessage rs)
        {
        }
        #endregion
    }
}
