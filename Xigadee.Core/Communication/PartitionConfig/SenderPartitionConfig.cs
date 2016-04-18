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
        static readonly SenderPartitionConfig[] mDefault;

        static SenderPartitionConfig()
        {
            mDefault = Init(1).ToArray();
        }

        public SenderPartitionConfig(int priority):base(priority)
        {
        }

        public static IEnumerable<SenderPartitionConfig> Init(params int[] priority)
        {
            foreach(int p in priority)
                yield return new SenderPartitionConfig(p);
        }

        public static SenderPartitionConfig[] Default
        {
            get
            {
                return mDefault;
            }
        }
    }
}
