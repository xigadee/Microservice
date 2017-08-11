using System;

namespace Xigadee
{
    /// <summary>
    /// THis helper 
    /// </summary>
    public static class CommunicationBridgeHelper
    {
        /// <summary>
        /// Gets the dead letter listener from the incoming manual listener.
        /// </summary>
        /// <param name="sender">The m listener.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IListener GetDeadLetterListener(this ManualChannelSender sender)
        {
            var listener = new ManualChannelListener();

            
            throw new NotImplementedException();
        }
    }
}
