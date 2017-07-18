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
    /// These extension methods connect the service bus listeners in to the pipeline.
    /// </summary>
    public static partial class AzureExtensionMethods
    {
        public static C AttachAzureServiceBusQueueSender<C>(this C cpipe
            , string connectionName = null
            , IEnumerable<SenderPartitionConfig> priorityPartitions = null
            , string serviceBusConnection = null
            , Action<AzureSBQueueSender> onCreate = null)
            where C: IPipelineChannelOutgoing<IPipeline>
        {
            var component = new AzureSBQueueSender();
            Channel channel = cpipe.ToChannel(ChannelDirection.Outgoing);

            component.ConfigureAzureMessaging(
                  channel.Id
                , priorityPartitions ?? channel.Partitions.Cast<SenderPartitionConfig>()
                , null
                , connectionName ?? channel.Id
                , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
                );

            onCreate?.Invoke(component);

            cpipe.AttachSender(component, false);

            return cpipe;
        }
    }
}
