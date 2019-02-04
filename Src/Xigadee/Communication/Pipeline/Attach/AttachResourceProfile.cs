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
using Xigadee;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method attaches one or more resource profiles to an incoming channel.
        /// You can use syntax such as .AttachResourceProfile("Resource1","Resource2") as there is an impicit cast from string to ResourceProfile.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="profiles">The profile collection.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachResourceProfile<C>(this C cpipe
            , params ResourceProfile[] profiles)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            if (profiles == null)
                throw new ArgumentNullException($"{nameof(AttachResourceProfile)}: profiles cannot be null");

            profiles.ForEach((p) => cpipe.ToChannel(ChannelDirection.Incoming).ResourceProfiles.Add(p));

            return cpipe;
        }
    }
}
