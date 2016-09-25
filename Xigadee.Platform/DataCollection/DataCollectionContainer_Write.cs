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

        public void Write(LogEvent eventData)
        {
            mContainerLogger.EventSubmit(eventData, eventData.Level != LoggingLevel.Status);
        }

        public void Write(BoundaryEvent eventData)
        {

        }

        public void Write(PayloadEvent eventData)
        {

        }

        public void Write(MicroserviceStatistics eventData)
        {

        }

        public void Write(MetricEvent eventData)
        {

        }

        public void Write(EventSourceEvent eventData)
        {

        }
    }
}
