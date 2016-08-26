using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class EventSourceExtensionMethods
    {
        public static IEventSource AddEventSource(this ConfigurationPipeline pipeline, IEventSource eventSource)
        {
            return pipeline.Service.RegisterEventSource(eventSource);
        }

        public static IEventSource AddEventSource(this ConfigurationPipeline pipeline, Func<IEnvironmentConfiguration, IEventSource> eventSource)
        {
            return pipeline.Service.RegisterEventSource(eventSource(pipeline.Configuration));
        }
    }
}
