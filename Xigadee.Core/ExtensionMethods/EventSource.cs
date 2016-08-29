using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class EventSourceExtensionMethods
    {
        public static MicroservicePipeline AddEventSource(this MicroservicePipeline pipeline, IEventSource eventSource)
        {
            pipeline.Service.RegisterEventSource(eventSource);

            return pipeline;
        }

        public static MicroservicePipeline AddEventSource<E>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, E> creator, Action<E> assign)
            where E: IEventSource
        {
            var eSource = creator(pipeline.Configuration);
            assign?.Invoke(eSource);
            pipeline.Service.RegisterEventSource(eSource);
            return pipeline;
        }
    }
}
