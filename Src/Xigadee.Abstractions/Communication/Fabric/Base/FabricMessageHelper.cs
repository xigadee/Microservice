namespace Xigadee
{
    /// <summary>
    /// This static class is used to set the key properties to enable messages to be transmitted
    /// over the Azure Service Bus.
    /// </summary>
    public static class FabricMessageHelper
    {
        #region AssignMessageHelpers<C>(this AzureClientHolder<C,BrokeredMessage> client)
        /// <summary>
        /// This extension method set the Pack, Unpack and Signal functions for Azure Service Bus support.
        /// </summary>
        /// <typeparam name="C">The Client Entity type.</typeparam>
        /// <param name="client">The client to set.</param>
        public static void AssignMessageHelpers<C>(this ManualChannelClientHolder client)
        {
            client.MessagePack = FabricMessage.Pack;
            client.MessageUnpack = FabricMessage.Unpack;
            client.MessageSignal = MessageSignal;
        }
        #endregion

        #region MessageSignal(BrokeredMessage message, bool success)
        /// <summary>
        /// This helper method signals to the underlying fabric that the message has succeeded or failed.
        /// </summary>
        /// <param name="message">The fabric message.</param>
        /// <param name="success">The message success status.</param>
        public static void MessageSignal(FabricMessage message, bool success)
        {
            if (success)
                message.Complete();
            else
                message.Abandon();
        }
        #endregion
    }
}
