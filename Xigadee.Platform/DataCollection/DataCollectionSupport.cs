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

namespace Xigadee
{
    /// <summary>
    /// This enumeration is used to specify the type of data collection supported by the component.
    /// </summary>
    [Flags]
    public enum DataCollectionSupport:int
    {
        /// <summary>
        /// This is a default setting.
        /// </summary>
        None = 0,
        /// <summary>
        /// The data is a log message.
        /// </summary>
        Logger = 1,
        /// <summary>
        /// The data is a event source record.
        /// </summary>
        EventSource = 2,
        /// <summary>
        /// The data is a boundary log event.
        /// </summary>
        BoundaryLogger = 4,
        /// <summary>
        /// The data is telemetry.
        /// </summary>
        Telemetry = 8,
        /// <summary>
        /// The data is a dispatcher transit event.
        /// </summary>
        Dispatcher = 16,
        /// <summary>
        /// The data is Microservice statistics
        /// </summary>
        Statistics = 32,
        /// <summary>
        /// The data is a resource event.
        /// </summary>
        Resource = 64,
        /// <summary>
        /// The data is a customer event.
        /// </summary>
        Custom = 128
    }
}
