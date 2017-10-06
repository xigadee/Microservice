using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This agent is used by the Event Hub fabric.
    /// </summary>
    /// <seealso cref="Xigadee.CommunicationBridgeAgent" />
    public class AzureEventHubsBridgeAgent : CommunicationBridgeAgent
    {
        public AzureEventHubsBridgeAgent()
        {

        }

        public override IListener GetListener()
        {
            throw new NotImplementedException();
        }

        public override ISender GetSender()
        {
            throw new NotImplementedException();
        }
    }
}
