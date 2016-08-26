using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class CommandExtensionMethods
    {
        public static MicroservicePipeline AddCommand<C>(this MicroservicePipeline pipeline
            , C command)
            where C: ICommand
        {
            pipeline.Service.RegisterCommand(command);

            return pipeline;
        }

        public static MicroservicePipeline AddCommand<C>(this MicroservicePipeline pipeline
            , Func<IEnvironmentConfiguration, C> creator
            //, ChannelPipelineIncoming channelIncoming = null
            )
            where C: ICommand
        {
            var command = creator(pipeline.Configuration);

            pipeline.AddCommand(creator);

            return pipeline;
        }


    }
}
