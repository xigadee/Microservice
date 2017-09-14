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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the standard service implementation.
    /// </summary>
    public interface IService 
    {
        /// <summary>
        /// This event is fired when the service status changes.
        /// </summary>
        event EventHandler<StatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Starts the service and returns immediately.
        /// </summary>
        void Start();

        /// <summary>
        /// This method stops a service.
        /// </summary>
        void Stop();

        /// <summary>
        /// This method allows the statistics to be retrieved as a generic format.
        /// </summary>
        /// <returns></returns>
        StatusBase StatisticsGet();
    }

    /// <summary>
    /// This class holds the status change.
    /// </summary>
    [DebuggerDisplay("{StatusOld}->{StatusNew} {Message}")]
    public class StatusChangedEventArgs: EventArgs
    {
        /// <summary>
        /// The previous service status.
        /// </summary>
        public ServiceStatus StatusOld { get; set; }
        /// <summary>
        /// The new service status.
        /// </summary>
        public ServiceStatus StatusNew { get; set; }
        /// <summary>
        /// This is an optional message describing the reason for the status change.
        /// </summary>
        public string Message { get; set; }
    }

}
