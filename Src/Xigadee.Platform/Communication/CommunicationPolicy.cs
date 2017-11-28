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
    /// This is the policy that defines how the communication component operates.
    /// </summary>
    public class CommunicationPolicy:PolicyBase
    {
        /// <summary>
        /// Set outgoing routing information to lower-case. This is important as messaging protocols such as
        /// Service Bus can be case sensitive when running subscription filters.
        /// </summary>
        public bool ServiceMessageHeaderConvertToLowercase { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the TransmissionPayload trace flag should be set to true.
        /// </summary>
        public bool TransmissionPayloadTraceEnabled { get; set; }
        /// <summary>
        /// This is the default time that a process submitted from a listener can execute for. The default value is 30 seconds.
        /// </summary>
        public TimeSpan? ListenerRequestTimespan { get; set; } = null;
        /// <summary>
        /// This is the default boundary logging status. When the specific status is not set, this value 
        /// will be used. The default setting is false.
        /// </summary>
        public bool BoundaryLoggingActiveDefault { get; set; }
        /// <summary>
        /// This property specifies that channel can be created automatically if they do not exist.
        /// If this is set to false, an error will be generated when a message is sent to a channel
        /// that has not been explicitly created.
        /// </summary>
        public bool AutoCreateChannels { get; set; } = true;
        /// <summary>
        /// This is the default algorithm used to assign poll cycles to the various listeners.
        /// </summary>
        public virtual IListenerClientPollAlgorithm ListenerClientPollAlgorithm { get; set; }  = new MultipleClientPollSlotAllocationAlgorithm();
    }
}