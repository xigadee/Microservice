using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class CommunicationExtensionMethods
    {
        public static IListener AddListener(this ConfigurationPipeline pipeline, IListener listener)
        {
            return pipeline.Service.RegisterListener(listener);
        }

        public static S AddListener<S>(this ConfigurationPipeline pipeline, Func<IEnvironmentConfiguration, S> creator)
            where S : IListener
        {
            return (S)pipeline.AddListener(creator(pipeline.Configuration));
        }

        public static ISender AddSender(this ConfigurationPipeline pipeline, ISender sender)
        {
            return pipeline.Service.RegisterSender(sender);
        }

        public static S AddSender<S>(this ConfigurationPipeline pipeline, Func<IEnvironmentConfiguration, S> creator)
            where S: ISender
        {
            return (S)pipeline.AddSender(creator(pipeline.Configuration));
        }
    }
}
