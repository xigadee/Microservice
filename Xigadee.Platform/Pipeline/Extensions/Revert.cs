using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        public static MicroservicePipeline Revert<C>(this C cpipe, Action<C> assign = null)
            where C : ChannelPipelineBase
        {
            assign?.Invoke(cpipe);

            return cpipe.Pipeline;
        }
    }
}
