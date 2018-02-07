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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to identify a message sender.
    /// </summary>
    public interface ISender: IMessaging
    {
        /// <summary>
        /// This is a list of active clients.
        /// </summary>
        IEnumerable<ClientHolder> ListenerClients { get; }

        /// <summary>
        /// Transmits the message.
        /// </summary>
        /// <param name="message">The message.</param>
        Task SenderTransmit(TransmissionPayload message);

        /// <summary>
        /// This method returns true if the sender supports the channel.
        /// </summary>
        /// <param name="channel">The channelId to validate.</param>
        /// <returns>Returns true if the sender can handle the channel.</returns>
        bool SupportsChannel(string channel);

        /// <summary>
        /// This contains the sender partitions.
        /// </summary>
        List<SenderPartitionConfig> PriorityPartitions { get; set; }

    }

}
