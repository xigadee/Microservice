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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ApiProviderAuthApim: IApiProviderAuthBase
    {

        #region ApiKey
        /// <summary>
        /// The Azure api subscription key
        /// </summary>
        public string ApiKey { get; set; }
        #endregion        
        #region ApiTrace
        /// <summary>
        /// Set this to true to initiate an API trace event.
        /// </summary>
        public bool ApiTrace { get; set; }
        #endregion

        public void ProcessRequest(HttpRequestMessage rq)
        {
            //Add the azure management key when provided.
            if (!string.IsNullOrEmpty(ApiKey))
                rq.Headers.Add(ApimConstants.AzureSubscriptionKeyHeader, ApiKey);

            if (ApiTrace)
                rq.Headers.Add(ApimConstants.AzureTraceHeader, "true");
        }

        public void ProcessResponse(HttpResponseMessage rq)
        {
        }
    }
}
