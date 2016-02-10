#region using
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public static class EventHubHelper
    {
        public static EventHubDescription EventHubDescriptionGet(string tPath)
        {
            return new EventHubDescription(tPath)
            {
                
            };
        }

        /// <summary>
        /// This method creates the queue if it does not exist.
        /// </summary>
        public static void EventHubFabricInitialize(this AzureConnection conn, string name)
        {
            if (!conn.NamespaceManager.EventHubExists(name))
                conn.NamespaceManager.CreateEventHubIfNotExists(EventHubDescriptionGet(name));
        }

    }
}
