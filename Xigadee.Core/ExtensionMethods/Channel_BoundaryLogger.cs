using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelBoundaryLoggerExtensionMethods
    {
        public static P AppendBoundaryLogger<P,L>(this P cpipe
            , L boundaryLogger
            , Action<P,L> action = null
            )
            where P: ChannelPipelineBase
            where L: IBoundaryLogger
        {

            action?.Invoke(cpipe,boundaryLogger);
            cpipe.Channel.BoundaryLogger = boundaryLogger;

            return cpipe;
        }

        public static P AppendBoundaryLogger<P,L>(this P cpipe
            , Func<IEnvironmentConfiguration, L> creator
            , Action<P,L> action = null
            )
            where P : ChannelPipelineBase
            where L : IBoundaryLogger
        {
            var bLogger = creator(cpipe.Pipeline.Configuration);

            return cpipe.AppendBoundaryLogger(bLogger, action);
        }
    }
}
