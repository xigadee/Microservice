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

namespace Xigadee
{
    /// <summary>
    /// This enumeration specifies the behaviour that a command should follow when a command throws an error during execution.
    /// </summary>
    public enum ProcessRequestExceptionBehaviour
    {
        /// <summary>
        /// Do Nothing.
        /// </summary>
        DoNothing,
        /// <summary>
        /// The throw the exception to underlying dispatcher.
        /// </summary>
        ThrowException,
        /// <summary>
        /// The signal the payload as successful and send a 500 error response
        /// </summary>
        SignalSuccessAndSend500ErrorResponse,
        /// <summary>
        /// The signal the payload as failed so it will be retried by the underlying architecture and do nothing.
        /// </summary>
        SignalFailAndDoNothing,
        /// <summary>
        /// The custom behaviour that can be specified by overriding the processing method. This maps to ThrowException currently.
        /// </summary>
        Custom
    }
}
