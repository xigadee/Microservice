using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelBoundaryLoggerExtensionMethods
    {
        public static ChannelPipelineBase AppendBoundaryLogger(this ChannelPipelineBase cpipe
            , IBoundaryLogger boundaryLogger)
        {
            cpipe.Channel.BoundaryLogger = boundaryLogger;
            return cpipe;
        }

        public static ChannelPipelineBase AppendBoundaryLogger(this ChannelPipelineBase cpipe
            , Func<IEnvironmentConfiguration, IBoundaryLogger> creator
            , Action<IBoundaryLogger> action = null)
        {
            var bLogger = creator(cpipe.Pipeline.Configuration);

            action?.Invoke(bLogger);
            cpipe.Channel.BoundaryLogger = bLogger;

            return cpipe;
        }
    }
}
