using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class LoggingExtensionMethods
    {
        public static ILogger AddLogger(this MicroservicePipeline pipeline, ILogger logger)
        {
            return pipeline.Service.RegisterLogger(logger);
        }

        public static L AddLogger<L>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, L> logger)
            where L:ILogger
        {
            return (L)pipeline.Service.RegisterLogger(logger(pipeline.Configuration));
        }

    }
}
