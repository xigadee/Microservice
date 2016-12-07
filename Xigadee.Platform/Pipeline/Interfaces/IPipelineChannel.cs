using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the generic channel interface
    /// </summary>
    public interface IPipelineChannel<out P>: IPipelineChannel, IPipelineExtension<P>
        where P:IPipeline
    {
    }

    public interface IPipelineChannel
    {
        /// <summary>
        /// This is the channel definition.
        /// </summary>
        Channel Channel { get; }
    }

    public interface IPipelineChannelIncoming: IPipelineChannel { }
    public interface IPipelineChannelIncoming<out P>: IPipelineChannelIncoming,IPipelineChannel<P>
        where P : IPipeline
    {
    }

    public interface IPipelineChannelOutgoing: IPipelineChannel { }
    public interface IPipelineChannelOutgoing<out P>: IPipelineChannelOutgoing,IPipelineChannel<P>
        where P : IPipeline
    {
    }
}
