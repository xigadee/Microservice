using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class CommandExtensionMethods
    {
        public static C AddCommand<C>(this ConfigurationPipeline pipeline, C command)
            where C: ICommand
        {
            return (C)pipeline.Service.RegisterCommand(command);
        }

        public static C AddCommand<C>(this ConfigurationPipeline pipeline, Func<IEnvironmentConfiguration, C> creator)
            where C: ICommand
        {
            var command = creator(pipeline.Configuration);
            return pipeline.AddCommand(creator);
        }

        //public static ICommand AddCommand(this ConfigurationPipeline pipeline
        //    , ICommand command, IMicroserviceChannelIncoming incoming = null, IMicroserviceChannelOutgoing outgoing = null)
        //{
        //    return pipeline.Service.RegisterCommand(command);
        //}
    }
}
