using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class Microservice_Extensions
    {

        public static IMicroserviceChannelIncoming AddChannelIncoming(this Microservice service, string channelId)
        {
            return null;
        }
        public static IMicroserviceChannelOutgoing AddChannelOutgoing(this Microservice service, string channelId)
        {
            return null;
        }

        public static ILogger AddLogger(this Microservice service, ILogger logger)
        {
            return service.RegisterLogger(logger);
        }

        public static IEventSource AddEventSource(this Microservice service, IEventSource eventSource)
        {
            return service.RegisterEventSource(eventSource);
        }

        public static ICommand AddCommand(this Microservice service, ICommand command)
        {
            return service.RegisterCommand(command);
        }

        public static IPayloadSerializer AddPaylodSerializerDefaultJson(this Microservice service)
        {
            return service.RegisterPayloadSerializer(new JsonContractSerializer());
        }
        public static IPayloadSerializer AddPayloadSerializer(this Microservice service, IPayloadSerializer serializer)
        {
            return service.RegisterPayloadSerializer(serializer);
        }


    }


}
