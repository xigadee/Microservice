using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        public static MicroservicePipeline AddCommand<C>(this MicroservicePipeline pipeline
            , C command
            , Action<C> assignment = null
            , ChannelPipelineIncoming channelIncoming = null
            , ChannelPipelineOutgoing channelResponse = null
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
            , ChannelPipelineOutgoing channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C: ICommand
        {
            var command = creator(pipeline.Configuration);

            return pipeline.AddCommand(command, assignment, channelIncoming, channelResponse, channelMasterJobNegotiationIncoming);
        }

        public static ChannelPipelineIncoming AddCommand<C>(this ChannelPipelineIncoming cpipe, Action<C> assign = null)
            where C: ICommand, new()
        {
            var command = new C();
            assign?.Invoke(command);
            return cpipe.AddCommand(command);
        }

        public static ChannelPipelineIncoming AddCommand<C>(this ChannelPipelineIncoming cpipe
            , C command
            , Action<C> assignment = null
            , ChannelPipelineOutgoing channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : ICommand
        {
            cpipe.Pipeline.AddCommand(command, assignment, cpipe, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);

            return cpipe;
        }

        public static ChannelPipelineIncoming AddCommand<C>(this ChannelPipelineIncoming cpipe
            , Func<IEnvironmentConfiguration, C> creator
            , Action<C> assignment = null
            , ChannelPipelineOutgoing channelResponse = null
            , ChannelPipelineIncoming channelMasterJobNegotiationIncoming = null
            , ChannelPipelineOutgoing channelMasterJobNegotiationOutgoing = null
            )
            where C : ICommand
        {
            cpipe.Pipeline.AddCommand(creator, assignment, cpipe, channelResponse, channelMasterJobNegotiationIncoming, channelMasterJobNegotiationOutgoing);

            return cpipe;
        }
    }
}
