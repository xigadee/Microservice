using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class PassthroughExtensionMethods
    {
        public static MicroservicePipeline Inspect(this MicroservicePipeline pipeline
            , Action<IMicroservice> msAssign = null
            , Action<IEnvironmentConfiguration> cfAssign = null)
        {
            msAssign?.Invoke(pipeline.Service);
            cfAssign?.Invoke(pipeline.Configuration);

            return pipeline;
        }

        public static ChannelPipelineIncoming Inspect(this ChannelPipelineIncoming pipeline
            , Action<IMicroservice> msAssign = null
            , Action<IEnvironmentConfiguration> cfAssign = null
            , Action<Channel> cnAssign = null)
        {
            msAssign?.Invoke(pipeline.Pipeline.Service);
            cfAssign?.Invoke(pipeline.Pipeline.Configuration);
            cnAssign?.Invoke(pipeline.Channel);

            return pipeline;
        }

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
