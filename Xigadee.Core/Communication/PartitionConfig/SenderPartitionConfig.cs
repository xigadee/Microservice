#region using

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;
using System.Threading;
#endregion
namespace Xigadee
{
    public class SenderPartitionConfig: PartitionConfig
    {
        public static IEnumerable<SenderPartitionConfig> Init(params int[] priority)
        {
            foreach(int p in priority)
                yield return new SenderPartitionConfig(p);
        }


        public SenderPartitionConfig(int priority, TimeSpan? fabricMaxMessageLock = null) :base(priority, fabricMaxMessageLock)
        {
        }
    }
}
