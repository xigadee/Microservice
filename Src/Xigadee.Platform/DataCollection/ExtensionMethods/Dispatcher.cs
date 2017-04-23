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
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// These extension methods simplify the logging of complex data to a consistent framework.
    /// </summary>
    public static partial class DataCollectionExtensionMethods
    {
        public static void DispatcherPayloadComplete(this IDataCollection collector, TransmissionPayload payload, int delta, bool isSuccess)
        {
            collector.Write(new DispatcherEvent { Type = PayloadEventType.Complete, Payload = payload, Delta = delta, IsSuccess = isSuccess }, DataCollectionSupport.Dispatcher);
        }

        public static void DispatcherPayloadException(this IDataCollection collector, TransmissionPayload payload, Exception pex)
        {
            collector.Write(new DispatcherEvent { Type = PayloadEventType.Exception, Payload = payload, Ex = pex }, DataCollectionSupport.Dispatcher);
        }

        public static void DispatcherPayloadIncoming(this IDataCollection collector, TransmissionPayload payload)
        {
            collector.Write(new DispatcherEvent { Type = PayloadEventType.Incoming, Payload = payload }, DataCollectionSupport.Dispatcher);
        }

        public static void DispatcherPayloadUnresolved(this IDataCollection collector, TransmissionPayload payload, DispatcherRequestUnresolvedReason reason)
        {
            collector.Write(new DispatcherEvent { Type = PayloadEventType.Unresolved, Payload = payload, Reason = reason }, DataCollectionSupport.Dispatcher);
        }
    }
}
