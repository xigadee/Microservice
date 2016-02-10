using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ISender: IMessaging
    {
        /// <summary>
        /// Dispatches the message.
        /// </summary>
        /// <param name="message">The message.</param>
        Task ProcessMessage(TransmissionPayload message);

        /// <summary>
        /// This method returns true if the sender supports the channel.
        /// </summary>
        /// <param name="channel">The channelId to validate.</param>
        /// <returns>Returns true if the sender can handle the channel.</returns>
        bool SupportsChannel(string channel);

    }

}
