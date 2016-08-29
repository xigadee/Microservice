using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class SerializerExtensionMethods
    {
        public static MicroservicePipeline AddPayloadSerializerDefaultJson(this MicroservicePipeline pipeline)
        {
            var component = pipeline.Service.RegisterPayloadSerializer(new JsonContractSerializer());

            return pipeline;
        }

        public static MicroservicePipeline AddPayloadSerializer(this MicroservicePipeline pipeline, IPayloadSerializer serializer)
        {
            pipeline.Service.RegisterPayloadSerializer(serializer);

            return pipeline;
        }

        public static MicroservicePipeline AddPayloadSerializer(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, IPayloadSerializer> creator)
        {
            pipeline.Service.RegisterPayloadSerializer(creator(pipeline.Configuration));

            return pipeline;
        }
    }
}
