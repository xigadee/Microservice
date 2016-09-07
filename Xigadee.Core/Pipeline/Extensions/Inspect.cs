using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
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


    }
}
