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


using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Xigadee
{
    public static class ApiUtility
    {
        public static RepositorySettings BuildRepositorySettings(HttpActionContext aContext)
        {
            return BuildRepositorySettings(aContext.RequestContext, aContext.Request);
        }
        /// <summary>
        /// Builds a repository settings object based on the request and request context. Sets up the prefer values, APIM source etc
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RepositorySettings BuildRepositorySettings(HttpRequestContext requestContext, HttpRequestMessage request)
        {
            var settings = new RepositorySettings();

            IEnumerable<string> preferValues;
            //See: http://tools.ietf.org/html/rfc7240
            if (request.Headers.TryGetValues("Prefer", out preferValues))
            {
                var preferences = preferValues.First().Split(',');

                foreach (var keyValuePair in preferences.Select(p => p.Split('=')).Where(kvp => kvp.Length > 1))
                {
                    settings.Prefer.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
                }
            }

            AddHeader(request, settings, ApimConstants.AzureSubscriptionKeyHeader);

            ApimPrincipal principal = requestContext.Principal as ApimPrincipal;

            if (principal != null)
            {
                var identity = principal.Identity as ApimIdentity;
                if (identity != null)
                {
                    settings.Source = identity.Source;
                    settings.SourceId = identity.Id;
                    settings.SourceName = identity.Name;
                }
            }

            return settings;
        }

        private static void AddHeader(HttpRequestMessage requestMessage, RepositorySettings repositorySettings, string header)
        {
            IEnumerable<string> values;
            if (!requestMessage.Headers.TryGetValues(header, out values))
                return;

            repositorySettings.Headers[header] = string.Join(";", values);
        }
    }
}
