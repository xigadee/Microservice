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
        /// This is the algorithm used to assign poll cycles to the various listeners.
        /// </summary>
        public virtual IListenerClientPollAlgorithm ListenerClientPollAlgorithm { get; set; }  = new MultipleClientPollSlotAllocationAlgorithm();
    }
}