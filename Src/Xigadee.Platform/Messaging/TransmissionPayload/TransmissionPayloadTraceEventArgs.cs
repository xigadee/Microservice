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
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class holds the trace information for the payload.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    [DebuggerDisplay("{Extent} @ {Source} - {Message}")]
    public class TransmissionPayloadTraceEventArgs: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmissionPayloadTraceEventArgs"/> class.
        /// </summary>
        /// <param name="start">The tickcount start.</param>
        /// <param name="message">The message.</param>
        /// <param name="source">The optional source parameter.</param>
        public TransmissionPayloadTraceEventArgs(int start, string message = null, string source = null)
        {
            Extent = ConversionHelper.DeltaAsTimeSpan(start, Environment.TickCount);
            Source = source;
            Message = message;
        }

        /// <summary>
        /// Gets the extent from when the TransmissionPayload was created.
        /// </summary>
        public TimeSpan? Extent { get; }
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { get; set;}
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set;}

    }
}
