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
//using Xigadee;

//namespace Test.Xigadee
//{
//    /// <summary>
//    /// This is the client populator. This is used to configure and start the Microservice.
//    /// </summary>
//    internal class PopulatorClient: PopulatorConsoleBase<MicroserviceClient>
//    {
//        protected override void RegisterCommands()
//        {
//            base.RegisterCommands();

//            var cacheManager = new RedisCacheManager<Guid, MondayMorningBlues>(Config.RedisCacheConnection(), true, new EntityTransformHolder<Guid, MondayMorningBlues>(true) { KeyDeserializer = s => new Guid(s) });

//            Persistence = (IRepositoryAsync<Guid, MondayMorningBlues>)Service.Commands.Register(
//                new PersistenceMessageInitiator<Guid, MondayMorningBlues>(cacheManager)
//                {
//                    ChannelId = Channels.TestB
//                    , ResponseChannelId = Channels.Interserve
//                });

//            Service.Commands.Register(
//                new PersistenceMessageInitiator<Guid, Blah>()
//                {
//                      ChannelId = Channels.TestB
//                    , ResponseChannelId = Channels.Interserve
//                });
//        }

//        protected override void RegisterCommunication()
//        {
//            base.RegisterCommunication();

//            //Service.RegisterListener(new AzureSBTopicListener(
//            //      Channels.Interserve
//            //    , Config.ServiceBusConnection()
//            //    , Channels.Interserve
//            //    , deleteOnStop: false
//            //    , listenOnOriginatorId: true
//            //    , priorityPartitions: ListenerPartitionConfig.Init(0, 1)));

//            //Service.RegisterListener(new AzureSBQueueListener(
//            //      Channels.TestA
//            //    , Config.ServiceBusConnection()
//            //    , Channels.TestA
//            //    , ListenerPartitionConfig.Init(0, 1)
//            //    , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));

//            //Service.RegisterSender(new AzureSBQueueSender(
//            //      Channels.TestB
//            //    , Config.ServiceBusConnection()
//            //    , Channels.TestB
//            //    , SenderPartitionConfig.Init(0, 1)));

//        }
//    }
//}
