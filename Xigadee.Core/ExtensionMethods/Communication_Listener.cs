using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class CommunicationListenerExtensionMethods
    {
        public static IListener AddListener(this MicroservicePipeline pipeline, IListener listener)
        {
            return pipeline.Service.RegisterListener(listener);
        }

        public static S AddListener<S>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where S : IListener
        {
            var listener = creator(pipeline.Configuration);

            action?.Invoke(listener);

            return (S)pipeline.AddListener(listener);
        }

        //public static S AddListener<S>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
        //    where S : IListener
        //{
        //    var listener = creator(pipeline.Configuration);

        //    action?.Invoke(listener);

        //    return (S)pipeline.AddListener(listener);
        //}

    }
}
