//#region Copyright
//// Copyright Hitachi Consulting
//// 
//// Licensed under the Apache License, Version 2.0 (the "License");
//// you may not use this file except in compliance with the License.
//// You may obtain a copy of the License at
//// 
////    http://www.apache.org/licenses/LICENSE-2.0
//// 
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// limitations under the License.
//#endregion

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web.Http.Filters;
//using System.Web.Http.Results;

//namespace Xigadee
//{
//    /// <summary>
//    /// This is a test Apim authorization implementation.
//    /// </summary>
//    public class WebApiApimAuthorization : IAuthenticationFilter
//    {
//        private readonly ApimConnector mConnector;
//        private readonly Timer mApimRefreshTimer;

//        public WebApiApimAuthorization(string apiId, string apiKey, Uri apiUri)
//        {
//            mConnector = new ApimConnector(apiId, apiKey, apiUri, TimeSpan.FromDays(7));
//            mConnector.Load();

//            mApimRefreshTimer = new Timer(RefreshApim, null, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
//        }

//        private void RefreshApim(object state)
//        {
//            if (mConnector != null)
//                mConnector.Refresh();
//        }

//        public bool AllowMultiple
//        {
//            get { return false; }
//        }

//        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
//        {
//            IEnumerable<string> subKeys;
//            ApimPrincipal principal;
//            string subKey;
//            if (context.Request.Headers.TryGetValues(ApimConstants.AzureSubscriptionKeyHeader, out subKeys))
//            {
//                subKey = subKeys.FirstOrDefault();
//            }
//            else
//            {
//                subKey = context.Request.GetQueryNameValuePairs().FirstOrDefault(
//                        kvp => kvp.Key.Equals(ApimConstants.AzureSubscriptionKeyQueryString, StringComparison.InvariantCultureIgnoreCase)).Value;
//            }

//            if (!string.IsNullOrEmpty(subKey) && mConnector.TryGetValue(subKey, out principal))
//            {
//                context.Principal = principal;
//            }
//            else
//            {
//                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
//            }
//        }

//        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
//        {
//            return Task.FromResult(context.Result);
//        }
//    }
//}
