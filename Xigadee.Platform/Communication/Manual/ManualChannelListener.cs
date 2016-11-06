using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the manual tester, primarily used for testing.
    /// </summary>
    public class ManualChannelListener: MessagingListenerBase<ManualChannelConnection, ManualChannelMessage, ManualChannelClientHolder>
    {

        protected override void ClientsStart()
        {
            base.ClientsStart();
        }

        protected override ManualChannelClientHolder ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.ClientCreate = () => new ManualChannelConnection();

            return client;
        }
        /// <summary>
        /// This method injects a service message manually in to the Microservice.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="priority">The optional priority. The default is 1.</param>
        public void Inject(ServiceMessage message, int priority = 1)
        {
            var client = ClientResolve(priority);

            client.Inject(message);

        }
    }
}
