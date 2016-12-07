﻿#region Copyright
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
        public static IPipelineChannelOutgoing AttachSender(this IPipelineChannelOutgoing cpipe
            , ISender sender
            , bool setFromChannelProperties = true)
        {
            if (cpipe.Channel.InternalOnly)
                throw new ChannelInternalOnlyException(cpipe.Channel.Id, cpipe.Channel.Direction);

            if (setFromChannelProperties)
            {
                sender.ChannelId = cpipe.Channel.Id;
                sender.PriorityPartitions = cpipe.Channel.Partitions.Cast<SenderPartitionConfig>().ToList();
            }

            cpipe.Pipeline.Service.RegisterSender(sender);

            return cpipe;
        }

        public static IPipelineChannelOutgoing AttachSender<S>(this IPipelineChannelOutgoing cpipe
            , Func<IEnvironmentConfiguration, S> creator = null
            , Action<S> action = null
            , bool setFromChannelProperties = true)
            where S : ISender, new()
        {
            var sender = (creator==null)?new S():creator(cpipe.Pipeline.Configuration);

            action?.Invoke(sender);

            cpipe.AttachSender(sender, setFromChannelProperties);

            return cpipe;
        }
    }
}
