using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class CommunicationSenderExtensionMethods
    {

        public static MicroservicePipeline AddSender(this MicroservicePipeline pipeline, ISender sender)
        {
            pipeline.Service.RegisterSender(sender);

            return pipeline;
        }

        public static MicroservicePipeline AddSender<S>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where S : ISender
        {
            var sender = creator(pipeline.Configuration);

            action?.Invoke(sender);

            pipeline.AddSender(sender);

            return pipeline;
        }
    }
}
