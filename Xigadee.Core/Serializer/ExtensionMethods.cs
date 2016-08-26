using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class SerializerExtensionMethods
    {
        public static IPayloadSerializer AddPayloadSerializerDefaultJson(this MicroservicePipeline pipeline)
        {
            return pipeline.Service.RegisterPayloadSerializer(new JsonContractSerializer());
        }

        public static IPayloadSerializer AddPayloadSerializer(this MicroservicePipeline pipeline, IPayloadSerializer serializer)
        {
            return pipeline.Service.RegisterPayloadSerializer(serializer);
        }
    }
}
