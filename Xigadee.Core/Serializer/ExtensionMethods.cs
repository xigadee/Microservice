using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class SerializerExtensionMethods
    {
        public static IPayloadSerializer AddPayloadSerializerDefaultJson(this ConfigurationPipeline pipeline)
        {
            return pipeline.Service.RegisterPayloadSerializer(new JsonContractSerializer());
        }

        public static IPayloadSerializer AddPayloadSerializer(this ConfigurationPipeline pipeline, IPayloadSerializer serializer)
        {
            return pipeline.Service.RegisterPayloadSerializer(serializer);
        }
    }
}
