using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class is used to configure the sender channels.
    /// </summary>
    [DebuggerDisplay("Sender {Priority}")]
    public class SenderPartitionConfig: PartitionConfig
    {
        #region Static methods
        /// <summary>
        /// This static method sets the priority channels for the integers passed at their default settings.
        /// </summary>
        /// <param name="priority">The priority list, i.e. 0,1</param>
        /// <returns>Returns a list of sender configs.</returns>
        public static IEnumerable<SenderPartitionConfig> Init(params int[] priority)
        {
            foreach (int p in priority)
                yield return new SenderPartitionConfig(p);
        }

        /// <summary>
        /// Implicitly converts an int to a ListenerPartitionConfig.
        /// </summary>
        /// <param name="id">The priority.</param>
        public static implicit operator SenderPartitionConfig(int id)
        {
            return new SenderPartitionConfig(id);
        } 
        #endregion

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="priority">The specific integer priority.</param>
        /// <param name="fabricMaxMessageLock">The maximum message lock time allowed when sending a message. The default time is 4.5 seconds.</param>
        public SenderPartitionConfig(int priority, TimeSpan? fabricMaxMessageLock = null) :base(priority, fabricMaxMessageLock)
        {
        }
    }
}
