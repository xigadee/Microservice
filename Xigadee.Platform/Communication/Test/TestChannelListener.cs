using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TestChannelListener: MessagingListenerBase<TestChannelConnection, TestChannelMessage, TestChannelClientHolder>
    {
        protected override void StartInternal()
        {
            base.StartInternal();
        }
    }
}
