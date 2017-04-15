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
        public static C AttachPersistenceInternalService<C,K,E>(this C cpipe
            , out PersistenceInternalService<K,E> command
            , string outgoingChannel
            , int startupPriority = 90
            , ICacheManager<K,E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            var ms = cpipe.ToMicroservice();
   
            command = new PersistenceInternalService<K, E>(cacheManager: cacheManager, defaultRequestTimespan: defaultRequestTimespan)
            {
                  ResponseChannelId = cpipe.Channel.Id
                , ChannelId = outgoingChannel
            };

            cpipe.Pipeline.AddCommand(command, startupPriority);

            return cpipe;
        }
    }
}
