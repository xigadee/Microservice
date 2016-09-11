using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// These extensions allow services to be registered as part of a pipeline
    /// </summary>
    public static partial class CorePipelineExtensions
    {
        public static MicroservicePipeline AddSharedService<I>(this MicroservicePipeline pipeline, I service, string serviceName = null, Action<I> action = null) where I : class
        {
            action?.Invoke(service);

            if (!pipeline.Service.SharedServices.RegisterService<I>(service, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }

        public static MicroservicePipeline AddSharedService<I>(this MicroservicePipeline pipeline, Lazy<I> service, string serviceName = null) where I : class
        {
            if (!pipeline.Service.SharedServices.RegisterService<I>(service, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }
    }
}
