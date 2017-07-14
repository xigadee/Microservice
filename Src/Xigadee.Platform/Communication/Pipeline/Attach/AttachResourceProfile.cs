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
        /// This pipeline method attaches a resource profile to an incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="profile">The resource profile.</param>
        /// <param name="action">An optional action.</param>
        /// <returns></returns>
        public static C AttachResourceProfile<C>(this C cpipe
            , ResourceProfile profile
            , Action<ResourceProfile> action = null)
            where C: IPipelineChannelIncoming<IPipeline>
        {
            if (profile == null)
                throw new ArgumentNullException($"{nameof(AttachResourceProfile)}: profile cannot be null");

            cpipe.AttachResourceProfile((c) => profile, action);

            return cpipe;
        }


        public static C AttachResourceProfile<C>(this C cpipe
            , Func<IEnvironmentConfiguration, ResourceProfile> creator
            , Action<ResourceProfile> action = null)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            if (creator == null)
                throw new ArgumentNullException($"{nameof(AttachResourceProfile)}: creator cannot be null");

            var profile = creator(cpipe.Pipeline.Configuration);

            action?.Invoke(profile);

            cpipe.ChannelResolve(ChannelDirection.Incoming).ResourceProfiles.Add(profile);

            return cpipe;
        }

        public static C AttachResourceProfile<C>(this C cpipe
            , IEnumerable<ResourceProfile> profiles)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            if (profiles == null)
                throw new ArgumentNullException($"{nameof(AttachResourceProfile)}: profiles cannot be null");

            profiles.ForEach((p) => cpipe.ChannelResolve(ChannelDirection.Incoming).ResourceProfiles.Add(p));

            return cpipe;
        }
    }
}
