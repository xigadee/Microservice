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
    /// This is the base interface shared by both listeners and senders.
    /// </summary>
    public interface IMessaging
    {
        /// <summary>
        /// This is a list of clients for the listener or sender.
        /// </summary>
        IEnumerable<ClientHolder> Clients { get; }

        /// <summary>
        /// This is the channel for the messaging agent.
        /// </summary>
        string ChannelId { get;set; }

        /// <summary>
        /// This is the boundary logger used by the service.
        /// </summary>
        IDataCollection Collector { get; set; }

        /// <summary>
        /// This property specifies whether the boundary logging is active for the messaging component..
        /// </summary>
        bool? BoundaryLoggingActive { get; set; }
    }
}
