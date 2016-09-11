using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ISupportedMessageTypes
    {
        /// <summary>
        /// This event is used to signal a change of registered commands.
        /// </summary>
        event EventHandler<SupportedMessagesChange> OnCommandChange;
        /// <summary>
        /// This is the supported message types for the listener.
        /// </summary>
        List<MessageFilterWrapper> SupportedMessages { get; }
    }

    public class SupportedMessagesChange
    {
        public List<MessageFilterWrapper> Messages { get; set; }
    }
}
