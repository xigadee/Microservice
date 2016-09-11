using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee.Pipeline.Extensions
{
    public static partial class CorePipelineExtensions
    {
        public static IMicroservice ToMicroservice(this MicroservicePipeline pipeline)
        {
            return pipeline.Service;
        }

        public static IEnvironmentConfiguration ToConfiguration(this MicroservicePipeline pipeline)
        {
            return pipeline.Configuration;
        }

        public static Channel ToChannel(this ChannelPipelineIncoming cpipe)
        {
            return cpipe.Channel;
        }
    }
}
