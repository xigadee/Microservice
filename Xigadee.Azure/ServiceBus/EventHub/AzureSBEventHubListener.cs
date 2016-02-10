#region using
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public class AzureSBEventHubListener : AzureSBListenerBase<EventHubClient, EventData>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The channelId used to identify internally the comms layer.</param>
        /// <param name="connectionString">The Azure Service bus connection string.</param>
        /// <param name="connectionName"></param>
        /// <param name="defaultTimeout"></param>
        /// <param name="isDeadLetterListener"></param>
        public AzureSBEventHubListener(string channelId, string connectionString, string connectionName, IEnumerable<ResourceProfile> resourceProfiles = null)
            : base(channelId, connectionString, connectionName, ListenerPartitionConfig.Default, resourceProfiles:resourceProfiles)
        {
        } 
        #endregion


    }
}
