//#region Copyright
//// Copyright Hitachi Consulting
//// 
//// Licensed under the Apache License, Version 2.0 (the "License");
//// you may not use this file except in compliance with the License.
//// You may obtain a copy of the License at
//// 
////    http://www.apache.org/licenses/LICENSE-2.0
//// 
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// limitations under the License.
//#endregion

//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Xigadee
//{
//    /// <summary>
//    /// These extension methods connect the service bus components in to the pipeline.
//    /// </summary>
//    public static partial class AzureServiceBusExtensionMethods
//    {

//        /// <summary>
//        /// Attaches the azure service bus queue listener.
//        /// </summary>
//        /// <typeparam name="C"></typeparam>
//        /// <param name="cpipe">The cpipe.</param>
//        /// <param name="connectionName">Name of the connection.</param>
//        /// <param name="serviceBusConnection">The service bus connection.</param>
//        /// <param name="isDeadLetterListener">if set to <c>true</c> [is dead letter listener].</param>
//        /// <param name="mappingChannelId">The mapping channel identifier.</param>
//        /// <param name="priorityPartitions">The priority partitions.</param>
//        /// <param name="resourceProfiles">The resource profiles.</param>
//        /// <param name="onCreate">The on create.</param>
//        /// <returns></returns>
//        public static C AttachAzureServiceBusQueueListener<C>(this C cpipe
//            , string connectionName = null
//            , string serviceBusConnection = null
//            , bool isDeadLetterListener = false
//            , string mappingChannelId = null
//            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
//            , IEnumerable<ResourceProfile> resourceProfiles = null
//            , Action<AzureServiceBusQueueListener> onCreate = null)
//            where C: IPipelineChannelIncoming<IPipeline>
//        {

//            //var component = new AzureServiceBusQueueListener();
//            //Channel channel = cpipe.ToChannel(ChannelDirection.Incoming);

//            //component.ConfigureAzureMessaging(
//            //      channel.Id
//            //    , priorityPartitions ?? channel.Partitions.Cast<ListenerPartitionConfig>()
//            //    , resourceProfiles
//            //    , connectionName ?? channel.Id
//            //    , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
//            //    );

//            //component.IsDeadLetterListener = isDeadLetterListener;

//            //onCreate?.Invoke(component);

//            //cpipe.AttachListener(component, setFromChannelProperties: false);

//            return cpipe;
//        }




//    }
//}
