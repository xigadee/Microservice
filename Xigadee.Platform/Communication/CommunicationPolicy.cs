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
        public CommunicationPolicy()
        {
            ClientPriorityRecaculateFrequency = TimeSpan.FromMinutes(10);
            PriorityAlgorithm = new DefaultClientPollSlotAllocationAlgorithm();
        }
        public TimeSpan ClientPriorityRecaculateFrequency { get; set; } 

        public virtual ClientPollSlotAllocationAlgorithm PriorityAlgorithm { get; set; } 
    }
}