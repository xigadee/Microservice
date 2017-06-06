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
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http.Filters;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the logging level.
    /// </summary>
    [Flags]
    public enum ApiBoundaryLoggingFilterLevel
    {
        /// <summary>
        /// No logging of any information.
        /// </summary>
        None = 0,

        Exception = 1,
        Request = 2,
        Response = 4,
        RequestContent = 8,
        ResponseContent = 16,

        All = 31
    }
}
