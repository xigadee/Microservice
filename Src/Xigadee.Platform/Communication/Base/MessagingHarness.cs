namespace Xigadee
{
    /// <summary>
    /// This harness is used to set the default configuration for the messaging based service.
    /// </summary>
    /// <typeparam name="L">The messaging class type.</typeparam>
    public abstract class MessagingHarness<L> : ServiceHarness<L>
        where L : class, IMessaging, IService
    {
        /// <summary>
        /// This method sets the channel id and boundary logging status.
        /// </summary>
        /// <param name="service">The messaging service.</param>
        protected override void Configure(L service)
        {
            base.Configure(service);
        }

        /// <summary>
        /// Configures the messaging harness.
        /// </summary>
        /// <param name="configuration">The environment configuration.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="boundaryLoggingActive">Sets boundary logging as active.</param>
        public virtual void Configure(IEnvironmentConfiguration configuration, string channelId
            , bool boundaryLoggingActive = true)
        {
            Service.ChannelId = channelId;
            Service.BoundaryLoggingActive = boundaryLoggingActive;
        }
    }
}
