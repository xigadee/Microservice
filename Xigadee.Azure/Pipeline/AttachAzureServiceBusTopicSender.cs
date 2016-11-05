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
        public static ChannelPipelineOutgoing AttachAzureServiceBusTopicSender(this ChannelPipelineOutgoing cpipe
            , string connectionName
            , string serviceBusConnection = null
            , IEnumerable<SenderPartitionConfig> priorityPartitions = null
            , Action<AzureSBTopicSender> onCreate = null
            , bool setFromChannelProperties = true
            )
        {
            if (connectionName == null)
                throw new ArgumentNullException("connectionName cannot be null.");

            var component = new AzureSBTopicSender();
                //  cpipe.Channel.Id
                //, cpipe.Pipeline.Configuration.ServiceBusConnectionValidate(serviceBusConnection)
                //, connectionName
                //, priorityPartitions ?? cpipe.Channel.Partitions.Cast<SenderPartitionConfig>().ToList()
                //);

            onCreate?.Invoke(component);

            cpipe.AttachSender(component, setFromChannelProperties);

            return cpipe;
        }
    }
}
