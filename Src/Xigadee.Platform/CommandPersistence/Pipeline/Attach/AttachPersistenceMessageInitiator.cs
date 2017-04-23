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
        public static C AttachPersistenceMessageInitiator<C,K,E>(this C cpipe
            , out PersistenceMessageInitiator<K,E> command
            , string responseChannel = null
            , int startupPriority = 90
            , ICacheManager<K,E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null
            , ProcessOptions routing = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            var ms = cpipe.ToMicroservice();
            
            responseChannel = responseChannel ?? $"AutoChannel{Guid.NewGuid().ToString("N").ToUpperInvariant()}";

            command = new PersistenceMessageInitiator<K, E>(cacheManager, defaultRequestTimespan)
            {
                  ResponseChannelId = responseChannel
                , ChannelId = cpipe.Channel.Id
                , RoutingDefault = routing
            };

            cpipe.Pipeline.AddCommand(command, startupPriority);

            return cpipe;
        }
    }
}
