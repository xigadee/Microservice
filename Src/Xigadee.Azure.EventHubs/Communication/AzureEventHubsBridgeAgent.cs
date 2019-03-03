using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This agent is used by the Event Hub fabric.
    /// </summary>
    /// <seealso cref="Xigadee.CommunicationFabricBridgeBase" />
    public class AzureEventHubsBridgeAgent : CommunicationFabricBridgeBase
    {
        public AzureEventHubsBridgeAgent():base(CommunicationFabricMode.NotSet)
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
