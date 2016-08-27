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
            , C command
            , Action<C> assignment = null
            , ChannelPipelineIncoming channelIncoming = null
            , ChannelPipelineIncoming channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C: ICommand
        {
            if (channelIncoming != null && command.ChannelIdAutoSet)
                command.ChannelId = channelIncoming.Channel.Id;

            if (channelResponse != null && command.ResponseChannelIdAutoSet)
                command.ResponseChannelId = channelResponse.Channel.Id;

            if (channelMasterJobNegotiationIncoming != null && command.MasterJobNegotiationChannelIdAutoSet)
                command.MasterJobNegotiationChannelIdIncoming = channelMasterJobNegotiationIncoming.Channel.Id;

            if (channelMasterJobNegotiationOutgoing != null && command.MasterJobNegotiationChannelIdAutoSet)
                command.MasterJobNegotiationChannelIdOutgoing = channelMasterJobNegotiationOutgoing.Channel.Id;

            assignment?.Invoke(command);
            pipeline.Service.RegisterCommand(command);
            return pipeline;
        }

        public static MicroservicePipeline AddCommand<C>(this MicroservicePipeline pipeline
            , Func<IEnvironmentConfiguration, C> creator
            , Action<C> assignment = null
            , ChannelPipelineIncoming channelIncoming = null
            , ChannelPipelineIncoming channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C: ICommand
        {
            var command = creator(pipeline.Configuration);

            return pipeline.AddCommand(command, assignment, channelIncoming, channelResponse, channelMasterJobNegotiationIncoming);

        }


    }
}
