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
        /// <summary>
        /// This extension method can be used to assign a registered encryption handler to the channel to ensure
        /// that the message payload in encrypted during transmission.
        /// </summary>
        /// <typeparam name="C">The pipeline channel extension type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="identifier">The encryption id.</param>
        /// <returns>Returns the pipeline.</returns>
        public static C AttachTransportPayloadDecryption<C>(this C cpipe, string identifier)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            if (!cpipe.Pipeline.Service.Security.HasEncryptionHandler(identifier))
                throw new EncryptionHandlerNotResolvedException(cpipe.Channel.Id, identifier);

            if (cpipe.Channel.EncryptionHandlerId != null)
                throw new ChannelEncryptionHandlerAlreadySetException(cpipe.Channel.Id);

            cpipe.Channel.EncryptionHandlerId = identifier;

            return cpipe;
        }
    }
}
