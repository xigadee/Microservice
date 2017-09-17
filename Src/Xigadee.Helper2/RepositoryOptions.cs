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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the repository request/response metadata.
    /// </summary>
    [DebuggerDisplay("ResponseCode={ResponseCode} IsSuccess={IsSuccess} IsFaulted={IsFaulted}")]
    public class RepositoryOptions
    {
        /// <summary>
        /// HTTP 404 code
        /// </summary>
        public const int ResponseNotFound = 404;
        /// <summary>
        /// HTTP 200 code
        /// </summary>
        public const int ResponseOK = 200;
        /// <summary>
        /// HTTP 403 code
        /// </summary>
        public const int ResponseForbidden = 403;
        /// <summary>
        /// HTTP 409 code
        /// </summary>
        public const int ResponseConflict = 409;
        /// <summary>
        /// HTTP 422 code
        /// </summary>
        public const int ResponseSignatureFailure = 422;
        /// <summary>
        /// HTTP 501 code
        /// </summary>
        public const int ResponseNotImplemented = 501;
        /// <summary>
        /// HTTP 406 code
        /// </summary>
        public const int ResponseNotAcceptable = 406;

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public RepositoryOptions()
        {
            RequestTransientFaultsRetryAttempts = 1;
            RequestTransientFaultsRetry = false;
            RequestEntityOnSignatureFailure = false;
        }

        /// <summary>
        /// This property is true if the request should handle transient faults.
        /// </summary>
        public bool RequestEntityOnSignatureFailure { get; set; }

        /// <summary>
        /// This property is true if the request should handle transient faults.
        /// </summary>
        public bool RequestTransientFaultsRetry { get; set; }
        /// <summary>
        /// This is the number of retry attempts 
        /// </summary>
        public int RequestTransientFaultsRetryAttempts { get; set; }

        /// <summary>
        /// This is the response code from SQL.
        /// </summary>
        public int ResponseCodeOriginal { get; set; }
        /// <summary>
        /// This is the response code from SQL.
        /// </summary>
        public int ResponseCode { get; set; }
        /// <summary>
        /// This is any exception caught.
        /// </summary>
        public Exception Ex { get; set; }
        /// <summary>
        /// This property is true if the request is faulted.
        /// </summary>
        public bool IsFaulted { get { return Ex != null; } }

        /// <summary>
        /// This property is true if the response is in the 200 range and has not faulted.
        /// </summary>
        public bool IsSuccess 
        { 
            get 
            { 
                return !IsFaulted && (ResponseCode >= 200 && ResponseCode < 300); 
            } 
        }

        /// <summary>
        /// This helper method throw the captured fault from the SQL provider.
        /// </summary>
        public void FaultValidate()
        {
            if (IsFaulted)
                throw Ex;
        }

        /// <summary>
        /// This helper method sets the response to OK.
        /// </summary>
        public void ResponseSetOK()
        {
            ResponseCode = ResponseOK;
        }
    }

}
