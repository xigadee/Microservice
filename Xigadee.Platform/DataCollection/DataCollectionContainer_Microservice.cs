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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public partial class DataCollectionContainer
    {

        public virtual void DispatcherPayloadException(TransmissionPayload payload, Exception pex)
        {

            //mDataCollection.LogException($"Unable to process {requestPayload?.Message} after {requestPayload?.Message?.FabricDeliveryCount} attempts", ex);

        }

        public virtual void DispatcherPayloadUnresolved(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason)
        {

        }

        public virtual void DispatcherPayloadIncoming(TransmissionPayload payload)
        {

        }

        public virtual void DispatcherPayloadComplete(TransmissionPayload payload, int delta, bool isSuccess)
        {

        }

        public virtual void MicroserviceStatisticsIssued(MicroserviceStatistics statistics)
        {

        }

    }
}
