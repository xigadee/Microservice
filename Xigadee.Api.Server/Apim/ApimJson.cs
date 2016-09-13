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
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xigadee
{
    /// <summary>
    /// Root Apim json returned from users operation
    /// </summary>
    internal class ApimUsers
    {
        [JsonProperty("value")]
        public List<ApimUserDetail> ApimUserDetails { get; set; }
    }

    /// <summary>
    /// Apim json returned from users operation
    /// </summary>
    internal class ApimUserDetail
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string state { get; set; }
        public string registrationDate { get; set; }
        public string note { get; set; }
    }

    /// <summary>
    /// Root Apim json returned from subscriptions operation
    /// </summary>
    internal class ApimSubscriptions
    {
        [JsonProperty("value")]
        public List<ApimSubscriptionDetail> ApimSubscriptionDetails { get; set; }
    }

    /// <summary>
    /// Apim json returned from subscriptions operation
    /// </summary>
    internal class ApimSubscriptionDetail
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string createdDate { get; set; }
        public string startDate { get; set; }
        public object expirationDate { get; set; }
        public string endDate { get; set; }
        public object notificationDate { get; set; }
        public string primaryKey { get; set; }
        public string secondaryKey { get; set; }
    }
}
