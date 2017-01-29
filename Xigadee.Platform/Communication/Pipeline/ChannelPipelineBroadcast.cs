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
    /// This is the outgoing channel.
    /// </summary>
    public class ChannelPipelineBroadcast<P>: ChannelPipelineBase<P>, IPipelineChannelBroadcast<P>
        where P: IPipeline
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channel">THe channel.</param>
        public ChannelPipelineBroadcast(P pipeline, Channel channelListener, Channel channelSender) : base(pipeline, null)
        {
            ChannelListener = channelListener;
            ChannelSender = channelSender;
        }


        /// <summary>
        /// This is the registered channel
        /// </summary>
        public Channel ChannelListener { get; }


        /// <summary>
        /// This is the registered channel
        /// </summary>
        public Channel ChannelSender { get; }
    }
}
