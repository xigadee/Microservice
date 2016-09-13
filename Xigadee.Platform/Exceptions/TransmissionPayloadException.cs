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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the DispatcherException
    /// </summary>
    public class TransmissionPayloadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        public TransmissionPayloadException() : base() { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TransmissionPayloadException(TransmissionPayload payload) : base(payload.Message.OriginatorKey) 
        {
            Payload = payload;
        }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public TransmissionPayloadException(TransmissionPayload payload, Exception ex) : base(payload.Message.OriginatorKey, ex) 
        {
            Payload = payload;
        }

        public TransmissionPayload Payload { get; private set; }
    }
}