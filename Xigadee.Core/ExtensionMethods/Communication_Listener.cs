using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class CommunicationListenerExtensionMethods
    {
        public static MicroservicePipeline AddListener(this MicroservicePipeline pipeline, IListener listener)
        {
            pipeline.Service.RegisterListener(listener);

            return pipeline;
        }

        public static MicroservicePipeline AddListener<S>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where S : IListener
        {
            var listener = creator(pipeline.Configuration);

            action?.Invoke(listener);

            pipeline.AddListener(listener);

            return pipeline;
        }

    }
}
