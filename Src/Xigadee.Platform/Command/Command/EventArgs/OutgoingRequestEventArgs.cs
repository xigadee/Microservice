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
    /// This class contains the change information for the command.
    /// </summary>
    public class OutgoingRequestEventArgs: CommandEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingRequestEventArgs"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        public OutgoingRequestEventArgs(OutgoingRequest request)
        {
            Request = request;
        }
        /// <summary>
        /// Gets the outgoing request.
        /// </summary>
        public OutgoingRequest Request { get; }
    }
}
