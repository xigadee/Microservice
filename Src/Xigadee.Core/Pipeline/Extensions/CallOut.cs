using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        public static MicroservicePipeline CallOut(this MicroservicePipeline pipe, Action<MicroservicePipeline> method)
        {
            method(pipe);

            return pipe;
        }

        public static P CallOut<P>(this P pipe, Action<P> method)
            where P : ChannelPipelineBase
        {
            method(pipe);

            return pipe;
        }
    }
}
