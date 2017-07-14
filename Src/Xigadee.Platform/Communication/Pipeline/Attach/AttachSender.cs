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
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method attaches a sender to the outgoing channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="sender">The sender to attach.</param>
        /// <param name="setFromChannelProperties">The default value is true. This sets the sender properties from the channel.</param>
        /// <returns>Returns the pipeline</returns>
        public static C AttachSender<C>(this C cpipe
            , ISender sender
            , bool setFromChannelProperties = true)
            where C: IPipelineChannelOutgoing<IPipeline>
        {
            Channel channel = cpipe.ChannelResolve(ChannelDirection.Outgoing);

            if (channel.InternalOnly)
                throw new ChannelInternalOnlyException(channel.Id, channel.Direction);

            if (setFromChannelProperties)
            {
                if (channel.Partitions == null)
                    throw new ChannelPartitionConfigNotSetException(channel.Id);

                sender.ChannelId = channel.Id;
                sender.PriorityPartitions = channel.Partitions.Cast<SenderPartitionConfig>().ToList();
                sender.BoundaryLoggingActive = channel.BoundaryLoggingActive;
            }

            cpipe.Pipeline.Service.Communication.RegisterSender(sender);

            return cpipe;
        }

        public static C AttachSender<C,S>(this C cpipe
            , Func<IEnvironmentConfiguration, S> creator = null
            , Action<S> action = null
            , bool setFromChannelProperties = true)
            where S : ISender, new()
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            var sender = (creator==null)?new S():creator(cpipe.Pipeline.Configuration);

            action?.Invoke(sender);

            cpipe.AttachSender(sender, setFromChannelProperties);

            return cpipe;
        }
    }
}
