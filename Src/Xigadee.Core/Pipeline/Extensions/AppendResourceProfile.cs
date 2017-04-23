using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        public static ChannelPipelineIncoming AppendResourceProfile(this ChannelPipelineIncoming cpipe
            , ResourceProfile profile
            , Action<ResourceProfile> action = null)
        {
            if (profile == null)
                throw new ArgumentNullException("profile cannot be null");

            cpipe.AppendResourceProfile((c) => profile, action);

            return cpipe;
        }

        public static ChannelPipelineIncoming AppendResourceProfile(this ChannelPipelineIncoming cpipe
            , string profileName
            , Action<ResourceProfile> action = null)
        {
            if (string.IsNullOrEmpty(profileName))
                throw new ArgumentNullException("profileName cannot be null or empty");

            cpipe.AppendResourceProfile((c) => new ResourceProfile(profileName), action);

            return cpipe;
        }

        public static ChannelPipelineIncoming AppendResourceProfile(this ChannelPipelineIncoming cpipe
            , Func<IEnvironmentConfiguration, ResourceProfile> creator
            , Action<ResourceProfile> action = null)
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
