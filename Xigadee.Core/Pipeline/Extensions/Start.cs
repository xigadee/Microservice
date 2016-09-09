using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        public static void Start(this MicroservicePipeline pipeline)
        {
            pipeline.Service.Start();
        }

        public static void Stop(this MicroservicePipeline pipeline)
        {
            pipeline.Service.Stop();
        }
    }
}
