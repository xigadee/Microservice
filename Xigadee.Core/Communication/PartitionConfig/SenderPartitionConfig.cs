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
            mDefault = Init(1);
        }

        public SenderPartitionConfig()
        {
        }

        public static SenderPartitionConfig[] Init(params int[] priority)
        {
            return Init<SenderPartitionConfig>(priority, (o, e) => { });
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
