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

namespace Xigadee
{
    /// <summary>
    /// This event args class is used to signal a command processing exception.
    /// </summary>
    /// <seealso cref="Xigadee.MicroserviceEventArgs" />
    public class ProcessRequestErrorEventArgs: MicroserviceEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRequestErrorEventArgs"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="ex">The exception.</param>
        public ProcessRequestErrorEventArgs(TransmissionPayload payload, Exception ex)
        {
            Payload = payload;
            Ex = ex;
        }
        /// <summary>
        /// The originating payload.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// The uncaught exception raised during command execution.
        /// </summary>
        public Exception Ex { get;  }
    }
}
