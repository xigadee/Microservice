using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class LoggingExtensionMethods
    {
        public static MicroservicePipeline AddLogger(this MicroservicePipeline pipeline, ILogger logger)
        {
            pipeline.Service.RegisterLogger(logger);

            return pipeline;
        }

        public static MicroservicePipeline AddLogger<L>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, L> creator, Action<L> action = null)
            where L:ILogger
        {
            var logger = creator(pipeline.Configuration);

            action?.Invoke(logger);

            pipeline.Service.RegisterLogger(logger);

            return pipeline;
        }

        public static MicroservicePipeline AddLogger<L>(this MicroservicePipeline pipeline, Action<L> action = null)
            where L : ILogger, new()
        {
            var logger = new L();

            action?.Invoke(logger);

            pipeline.Service.RegisterLogger(logger);

            return pipeline;
        }

    }
}
