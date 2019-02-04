
using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        public static IPipeline ToPipeline(this IPipelineBase pipe)
        {
            if (pipe is IPipelineExtension<IPipeline>)
                return ((IPipelineExtension<IPipeline>)pipe).Pipeline;
            else if (pipe is IPipeline)
                return (IPipeline)pipe;

            throw new ArgumentOutOfRangeException("pipe", "pipe must implement IPipelineExtension or IPipeline");

        }

        public static IMicroservice ToMicroservice(this IPipelineBase pipe)
        {
            return pipe.ToPipeline().Service;
        }

        public static IEnvironmentConfiguration ToConfiguration(this IPipelineBase pipe)
        {
            return pipe.ToPipeline().Configuration;
        }

    }
}
