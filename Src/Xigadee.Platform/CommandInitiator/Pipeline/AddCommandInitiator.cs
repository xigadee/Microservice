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

        static string ValidateOrCreateOutgoingChannel(IPipeline pipeline, string outgoingChannelId, Guid componentId, bool create)
        {
            outgoingChannelId = string.IsNullOrEmpty(outgoingChannelId?.Trim()) ? $"CommandInitiator{componentId.ToString("N").ToUpperInvariant()}":outgoingChannelId;

            if (pipeline.ToMicroservice().Communication.HasChannel(outgoingChannelId, ChannelDirection.Incoming))
                return outgoingChannelId;

            if (!create)
                throw new ChannelDoesNotExistException(outgoingChannelId, ChannelDirection.Incoming, pipeline.ToMicroservice().Id.Name);

            var outPipe = pipeline.AddChannelIncoming(outgoingChannelId, internalOnly:true);
            
            return outgoingChannelId;
        }

        /// <summary>
        /// This method adds a command initiator to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="command">The command initiator output.</param>
        /// <param name="startupPriority">The start up priority. The default is 90.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <param name="responseChannel">The incoming channel to attach the command initiator to.</param>
        /// <param name="createChannel">This will create the channel.</param>
        /// <returns>The pipeline.</returns>
        public static P AddCommandInitiator<P>(this P pipeline
            , out CommandInitiator command
            , int startupPriority = 90
            , TimeSpan? defaultRequestTimespan = null
            , string responseChannel = null
            , bool createChannel = true
            )
            where P:IPipeline
        {
            command = new CommandInitiator(defaultRequestTimespan);
            command.ResponseChannelId = ValidateOrCreateOutgoingChannel(pipeline, responseChannel, command.ComponentId, createChannel);
            return pipeline.AddCommand(command, startupPriority);
        }


        /// <summary>
        /// This method adds a command initiator to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="command">The command initiator output.</param>
        /// <param name="startupPriority">The start up priority. The default is 90.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <param name="responseChannel">The incoming channel to attach the command initiator to.</param>
        /// <param name="createChannel">This property specifies that the method should create a readonly channel just for the command initiator if the responseChannel is not found.</param>
        /// <returns>The pipeline.</returns>
        public static P AddICommandInitiator<P>(this P pipeline
            , out ICommandInitiator command
            , int startupPriority = 90
            , TimeSpan? defaultRequestTimespan = null
            , string responseChannel = null
            , bool createChannel = true
            )
            where P : IPipeline
        {
            CommandInitiator interim;
            pipeline.AddCommandInitiator(out interim, startupPriority, defaultRequestTimespan, responseChannel, createChannel);
            command = interim;
            return pipeline;
        }
    }
}
