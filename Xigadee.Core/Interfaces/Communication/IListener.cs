using System.Collections.Generic;
namespace Xigadee
{
    /// <summary>
    /// This is the base interface implemented by listeners.
    /// </summary>
    public interface IListener : IMessaging
    {
        void Update(List<MessageFilterWrapper> supported);
        /// <summary>
        /// This is the channel id that incoming messages will be mapped to.
        /// </summary>
        string MappingChannelId { get; }
    }
}
