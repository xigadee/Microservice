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
    /// This class can be used to simulate the sender functionality in unit test projects.
    /// </summary>
    public class ManualChannelSender:MessagingSenderBase<ManualFabricConnection, FabricMessage, ManualChannelClientHolder>
    {
        #region AzureConn
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        public ManualFabricBridge Fabric { get; set; }
        #endregion

        /// <summary>
        /// Occurs when a message is sent to the sender. This event is caught and is used to map to corresponding listeners.
        /// </summary>
        public event EventHandler<TransmissionPayload> OnProcess;

        private void ProcessInvoke(TransmissionPayload payload)
        {
            try
            {
                OnProcess?.Invoke(this, payload);
            }
            catch (Exception ex)
            {
                Collector?.LogException("ManualChannelSender/ProcessInvoke", ex);
            }
        }
        
        ///// <summary>
        ///// This is the default client create logic for the manual sender.
        ///// </summary>
        ///// <param name="partition">The partition collection.</param>
        ///// <returns>
        ///// Returns the client.
        ///// </returns>
        //protected override ManualChannelClientHolder ClientCreate(SenderPartitionConfig partition)
        //{
        //    var client = base.ClientCreate(partition);

        //    client.Name = mPriorityClientNamer(ChannelId, partition.Priority);

        //    client.IncomingAction = ProcessInvoke;

        //    return client;
        //}
    }
}
