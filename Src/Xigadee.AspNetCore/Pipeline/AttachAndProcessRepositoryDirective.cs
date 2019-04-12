using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public static partial class DirectiveHelperClass
    {
        /// <summary>
        /// This method creates and attaches a repository.
        /// </summary>
        /// <param name="channelIncoming">The incoming channel to attach the repository.</param>
        /// <param name="rd">The directive.</param>
        public static void AttachAndProcessRepositoryDirective(this IPipelineChannelIncoming<MicroservicePipeline> channelIncoming
            , RepositoryDirective rd)
        {

        }
    }
}
