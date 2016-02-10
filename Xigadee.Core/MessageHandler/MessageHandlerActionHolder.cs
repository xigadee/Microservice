#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to host external commands that can be called when a message of the
    /// specific channelId is received.
    /// </summary>
    public class MessageHandlerActionHolder : MessageHandlerBase<MessageHandlerStatistics>
    {
        public MessageHandlerActionHolder(Type contractType,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> deadLetterAction = null,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
        {
            string channelId, messageType, actionType;
            ServiceMessageHelper.ExtractContractInfo(contractType, out channelId, out messageType, out actionType);

            CommandRegister(channelId, messageType, actionType, action, deadLetterAction, exceptionAction);      
        }

        public MessageHandlerActionHolder(MessageFilterWrapper filter,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> deadLetterAction = null,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
        {
            CommandRegister(filter, action, deadLetterAction, exceptionAction); 
        }

        #region CommandsRegister()
        /// <summary>
        /// We don't specifically support and commands other than those implemented as part of 
        /// the action holders.
        /// </summary>
        public override void CommandsRegister()
        {
        } 
        #endregion
    }
}
