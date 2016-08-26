using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class EventSourceExtensionMethods
    {
        public static IEventSource AddEventSource(this MicroservicePipeline pipeline, IEventSource eventSource)
        {
            return pipeline.Service.RegisterEventSource(eventSource);
        }

        public static E AddEventSource<E>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, E> eventSource)
            where E: IEventSource
        {
            return (E)pipeline.Service.RegisterEventSource(eventSource(pipeline.Configuration));
        }
    }
}
