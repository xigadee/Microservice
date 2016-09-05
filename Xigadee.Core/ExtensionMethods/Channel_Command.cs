using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelCommandExtensionMethods
    {
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
