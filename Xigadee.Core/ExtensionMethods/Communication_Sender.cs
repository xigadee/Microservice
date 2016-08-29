using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class CommunicationSenderExtensionMethods
    {

        public static ISender AddSender(this MicroservicePipeline pipeline, ISender sender)
        {
            return pipeline.Service.RegisterSender(sender);
        }

        public static S AddSender<S>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where S : ISender
        {
            var sender = creator(pipeline.Configuration);

            action?.Invoke(sender);

            return (S)pipeline.AddSender(sender);
        }
    }
}
