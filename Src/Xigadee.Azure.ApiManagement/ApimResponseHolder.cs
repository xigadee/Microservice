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

//#region using
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Security.Principal;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
//using System.Web.Http.Results;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This class holds the response from the APIM management interface.
//    /// </summary>
//    public class ApimResponseHolder
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="ApimResponseHolder"/> class.
//        /// </summary>
//        public ApimResponseHolder()
//        {
//            Fields = new Dictionary<string, string>();
//        }
//        /// <summary>
//        /// Gets or sets a value indicating whether this instance is a success.
//        /// </summary>
//        public bool IsSuccess { get; set; }
//        /// <summary>
//        /// Gets or sets a value indicating whether this instance has timed out.
//        /// </summary>
//        public bool IsTimeout { get; set; }
//        /// <summary>
//        /// Gets or sets the Http response.
//        /// </summary>
//        public HttpResponseMessage Response { get; set; }
//        /// <summary>
//        /// Gets or sets the content.
//        /// </summary>
//        public string Content { get; set; }
//        /// <summary>
//        /// Gets or sets the exception.
//        /// </summary>
//        public Exception Ex { get; set; }
//        /// <summary>
//        /// Gets or sets the document identifier.
//        /// </summary>
//        public string DocumentId { get; set; }
//        /// <summary>
//        /// Gets or sets the etag.
//        /// </summary>
//        public string ETag { get; set; }
//        /// <summary>
//        /// Gets or sets the fields.
//        /// </summary>
//        public Dictionary<string, string> Fields { get; set; }
//    }
//}
