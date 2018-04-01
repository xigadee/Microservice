using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to identify a message sender.
    /// </summary>
    public interface ISender: IMessaging
    {
        /// <summary>
        /// This is a list of active clients.
        /// </summary>
        IEnumerable<ClientHolder> SenderClients { get; }

        /// <summary>
        /// Transmits the message.
        /// </summary>
        /// <param name="message">The message.</param>
        Task SenderTransmit(TransmissionPayload message);

        /// <summary>
        /// This method returns true if the sender supports the channel.
        /// </summary>
        /// <param name="channel">The channelId to validate.</param>
        /// <returns>Returns true if the sender can handle the channel.</returns>
        bool SenderSupportsChannel(string channel);

        /// <summary>
        /// This contains the sender partitions.
        /// </summary>
        List<SenderPartitionConfig> SenderPriorityPartitions { get; set; }

    }

}
