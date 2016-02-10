#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This interface is for handlers that change their commands dynamically.
    /// </summary>
    public interface IMessageHandlerDynamic: IMessageHandler
    {
        /// <summary>
        /// This event is used to signal a change of registered commands.
        /// </summary>
        event EventHandler<CommandChange> OnCommandChange;
    }

    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    public class CommandChange
    {
        public CommandChange(bool isRemoval, MessageFilterWrapper key)
        {
            IsRemoval = isRemoval;
            Key = key;
        }

        public bool IsRemoval { get; protected set; }

        public MessageFilterWrapper Key { get; protected set; }
    }

    /// <summary>
    /// This is the standard interface for a message handler.
    /// </summary>
    public interface IMessageHandler : 
        IPayloadSerializerConsumer, IServiceEventSource, IServiceLogger, IServiceOriginator
    {
        /// <summary>
        /// This method register the commands.
        /// </summary>
        void CommandsRegister();
        /// <summary>
        /// This method should return true if the handler support this specific message.
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <returns>Returns true if the message channelId is supported.</returns>
        bool SupportsMessage(ServiceMessageHeader messageHeader);
        /// <summary>
        /// This method processes the supported message.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="responseMessages">The response collection.</param>
        Task ProcessMessage(TransmissionPayload request, List<TransmissionPayload> responseMessages);
        /// <summary>
        /// Returns a list of message header types.
        /// </summary>
        /// <returns>Returns ServiceMessageHeader definition.</returns>
        List<MessageFilterWrapper> SupportedMessageTypes();
        /// <summary>
        /// This is a list of the handlers and their statistics.
        /// </summary>
        IEnumerable<CommandHandler> Items { get; }
        /// <summary>
        /// This is the handler priority used when starting and stopping services.
        /// </summary>
        int Priority { get; set; }
    }
}
