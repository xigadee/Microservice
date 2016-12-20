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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Xigadee
{
    public partial class AzureStorageDataCollector: DataCollectorBase<DataCollectorStatistics>
    {
        protected virtual void WriteLogEvent(LogEvent log)
        {

        }

        protected virtual void WriteEventSource(EventSourceEvent e)
        {

        }

        protected virtual void WriteStatistics(MicroserviceStatistics e)
        {

        }

        protected virtual void WriteDispatcherEvent(DispatcherEvent e)
        {

        }

        protected virtual void WriteBoundaryEvent(BoundaryEvent e)
        {

        }

        protected virtual void WriteTelemetryEvent(TelemetryEvent e)
        {

        }
    }
}
