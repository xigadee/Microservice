using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    [Flags]
    public enum CommunicationAgentCapabilities
    {
        Listener = 1,
        Sender = 3,
        Bidirectional = 3
    }
}
