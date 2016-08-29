using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelResourceProfileExtensionMethods
    {
        public static ChannelPipelineIncoming AppendResourceProfile(this ChannelPipelineIncoming cpipe
            , ResourceProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile cannot be null");

            cpipe.Channel.ResourceProfiles.Add(profile);

            return cpipe;
        }

        public static ChannelPipelineIncoming AppendResourceProfile(this ChannelPipelineIncoming cpipe
            , Func<IEnvironmentConfiguration, ResourceProfile> creator
            , Action<ResourceProfile> action)
        {
            if (creator == null)
                throw new ArgumentNullException("creator cannot be null");

            var profile = creator(cpipe.Pipeline.Configuration);

            action?.Invoke(profile);

            cpipe.Channel.ResourceProfiles.Add(profile);

            return cpipe;
        }

        public static ChannelPipelineIncoming AppendResourceProfile(this ChannelPipelineIncoming cpipe
            , IEnumerable<ResourceProfile> profiles)
        {
            if (profiles == null)
                throw new ArgumentNullException("profiles cannot be null");

            profiles.ForEach((p) => cpipe.Channel.ResourceProfiles.Add(p));

            return cpipe;
        }
    }
}
