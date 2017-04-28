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
using Microsoft.Practices.Unity;

namespace Xigadee
{
    public static partial class UnityWebApiExtensionMethods
    {
        /// <summary>
        /// This method attaches the persistence initiator and registers it with Unity for registration 
        /// with the controllers.
        /// </summary>
        /// <typeparam name="C">The Unity Web pipe type.</typeparam>
        /// <typeparam name="K">The key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="cpipe">The pipe.</param>
        /// <param name="command">The command created.</param>
        /// <param name="outgoingChannel">The channel to make requests.</param>
        /// <param name="startupPriority">The startup priority</param>
        /// <param name="cacheManager">The optional cache manager.</param>
        /// <param name="defaultRequestTimespan">The default timeout.</param>
        /// <returns>Returns the pipe.</returns>
        public static C AttachPersistenceMessageInitiatorUnity<C, K, E>(this C cpipe
            , out PersistenceClient<K, E> command
            , string outgoingChannel
            , int startupPriority = 90
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null
            )
            where C : IPipelineChannelOutgoing<IPipelineWebApiUnity>
            where K : IEquatable<K>
        {
            var ms = cpipe.ToMicroservice();

            command = new PersistenceClient<K, E>(cacheManager, defaultRequestTimespan)
            {
                ResponseChannelId = cpipe.Channel.Id
                , ChannelId = outgoingChannel
            };

            cpipe.Pipeline.AddCommand(command, startupPriority);

            cpipe.Pipeline.Unity.RegisterInstance(typeof(IRepositoryAsync<K, E>), command);

            return cpipe;
        }

    }
}
