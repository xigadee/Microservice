using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class LoggingExtensionMethods
    {
        public static ILogger AddLogger(this ConfigurationPipeline pipeline, ILogger logger)
        {
            return pipeline.Service.RegisterLogger(logger);
        }

        public static ILogger AddLogger(this ConfigurationPipeline pipeline, Func<IEnvironmentConfiguration, ILogger> logger)
        {
            return pipeline.Service.RegisterLogger(logger(pipeline.Configuration));
        }

    }
}
