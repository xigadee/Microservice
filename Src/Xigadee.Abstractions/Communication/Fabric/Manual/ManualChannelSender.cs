using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class can be used to simulate the sender functionality in unit test projects.
    /// </summary>
    public class ManualChannelSender:MessagingSenderBase<ManualFabricConnection, ManaualFabricMessage, ManualChannelClientHolder>
    {
        #region Fabric
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        public ManualCommunicationFabric Fabric { get; set; }
        #endregion

        /// <summary>
        /// Occurs when a message is sent to the sender. This event is caught and is used to map to corresponding listeners.
        /// </summary>
        public event EventHandler<TransmissionPayload> OnProcess;

        private void ProcessInvoke(TransmissionPayload payload)
        {
            try
            {
                OnProcess?.Invoke(this, payload);
            }
            catch (Exception ex)
            {
                Collector?.LogException("ManualChannelSender/ProcessInvoke", ex);
            }
        }

        ///// <summary>
        ///// This is the default client create logic for the manual sender.
        ///// </summary>
        ///// <param name="partition">The partition collection.</param>
        ///// <returns>
        ///// Returns the client.
        ///// </returns>
        //protected override ManualChannelClientHolder ClientCreate(SenderPartitionConfig partition)
        //{
        //    var client = base.ClientCreate(partition);

        //    client.Name = mPriorityClientNamer(ChannelId, partition.Priority);

        //    client.IncomingAction = ProcessInvoke;

        //    return client;
        //}
    }
}
