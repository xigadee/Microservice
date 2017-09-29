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

namespace Xigadee
{
    /// <summary>
    /// These extension methods connect the service bus listeners in to the pipeline.
    /// </summary>
    public static partial class AzureServiceBusExtensionMethods
    {

        /// <summary>
        /// Attaches the azure service bus topic sender.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="cpipe">The cpipe.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="serviceBusConnection">The service bus connection.</param>
        /// <param name="priorityPartitions">The priority partitions.</param>
        /// <param name="onCreate">The on create.</param>
        /// <param name="setFromChannelProperties">if set to <c>true</c> [set from channel properties].</param>
        /// <returns></returns>
        public static C AttachAzureServiceBusTopicSender<C>(this C cpipe
            , string connectionName = null
            , string serviceBusConnection = null
            , IEnumerable<SenderPartitionConfig> priorityPartitions = null
            , Action<AzureServiceBusTopicSender> onCreate = null
            , bool setFromChannelProperties = true
            )
            where C: IPipelineChannelOutgoing<IPipeline>
        {

            var component = new AzureServiceBusTopicSender();
            Channel channel = cpipe.ToChannel(ChannelDirection.Outgoing);

            component.ConfigureAzureMessaging(
                  channel.Id
                , priorityPartitions ?? channel.Partitions.Cast<SenderPartitionConfig>()
                , null
                , connectionName ?? channel.Id
                , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
                );

            onCreate?.Invoke(component);

            cpipe.AttachSender(component, setFromChannelProperties: setFromChannelProperties);

            return cpipe;
        }
    }
}
