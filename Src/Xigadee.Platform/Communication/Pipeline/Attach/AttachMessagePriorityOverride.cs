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

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method sets the process priority for incoming messages to the maximum settings for the Microservice.
        /// This is used to ensure that return messages from remote Microservices are prioritised over other messages.
        /// </summary>
        /// <typeparam name="C">The channel pipeline type.</typeparam>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <param name="overridePriority">The priority. By default this is null and is replaced by the maximum priority.</param>
        /// <returns>Returns the original pipeline.</returns>
        public static C AttachMessageProcessPriorityOverride<C>(this C cpipe, int? overridePriority = null)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            int priority = overridePriority ?? (cpipe.ToMicroservice().Policy.TaskManager.PriorityLevels - 1);

            cpipe.AttachMessageRedirectRule((p) => true, (p) => p.Message.ChannelPriority = priority);

            return cpipe;
        }

        /// <summary>
        /// This method sets the process priority for incoming reponse messages for a request that originated 
        /// from the Microservice to the maximum priority settings.
        /// This is used to ensure that return messages from remote Microservices are prioritised over other messages.
        /// </summary>
        /// <typeparam name="C">The channel pipeline type.</typeparam>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <param name="overridePriority">The priority. By default this is null and is replaced by the maximum priority.</param>
        /// <returns>Returns the original pipeline.</returns>
        public static C AttachMessagePriorityOverrideForResponse<C>(this C cpipe, int? overridePriority = null)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var ms = cpipe.ToMicroservice();
            int priority = overridePriority ?? (ms.Policy.TaskManager.PriorityLevels - 1);
            if (priority < 1)
                priority = 1;

            cpipe.AttachMessageRedirectRule((p) =>
            {
                return p.Message.CorrelationServiceId == ms.Id.ExternalServiceId;
            }
            , (p) => p.Message.ChannelPriority = priority);

            return cpipe;
        }
    }
}
