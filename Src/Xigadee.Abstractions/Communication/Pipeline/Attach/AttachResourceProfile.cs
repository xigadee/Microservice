using System;
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
