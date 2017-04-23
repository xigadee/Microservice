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
    /// This is the incoming channel.
    /// </summary>
    public class ChannelPipelineIncoming<P>: ChannelPipelineBase<P>, IPipelineChannelIncoming<P>
        where P: IPipeline
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channel">The channel.</param>
        public ChannelPipelineIncoming(P pipeline, Channel channel) : base(pipeline, channel)
        {

        }
    }
}
