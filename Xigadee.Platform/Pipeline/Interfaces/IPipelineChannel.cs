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
    public interface IPipelineChannel:IPipelineExtension
    {
        /// <summary>
        /// This is the channel definition.
        /// </summary>
        Channel Channel { get; }
    }

    public interface IPipelineChannelIncoming: IPipelineChannel
    {
    }

    public interface IPipelineChannelOutgoing: IPipelineChannel
    {
    }
}
