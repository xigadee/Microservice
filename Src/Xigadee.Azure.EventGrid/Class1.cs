//using Microsoft.Azure.EventGrid;
//using Microsoft.Azure.EventGrid.Models;
//using System;
//using System.Collections.Generic;

//namespace Xigadee.Azure.EventGrid
//{
//    public class Class1
//    {
//        public Class1()
//        {
//            string topicEndpoint = "https://<topic-name>.<region>-1.eventgrid.azure.net/api/events";
//            string topicKey = "<topic-key>";
//            string topicHostname = new Uri(topicEndpoint).Host;

//            TopicCredentials topicCredentials = new TopicCredentials(topicKey);
//            EventGridClient client = new EventGridClient(topicCredentials);

//            client.PublishEventsAsync(topicHostname, GetEventsList()).GetAwaiter().GetResult();
//            Console.Write("Published events to Event Grid.");
//        }


//        static IList<EventGridEvent> GetEventsList()
//        {
//            List<EventGridEvent> eventsList = new List<EventGridEvent>();
//            for (int i = 0; i < 1; i++)
//            {
//                eventsList.Add(new EventGridEvent()
//                {
//                    Id = Guid.NewGuid().ToString(),
//                    EventType = "Contoso.Items.ItemReceivedEvent",
//                    Data = new ContosoItemReceivedEventData()
//                    {
//                        ItemUri = "ContosoSuperItemUri"
//                    },

//                    EventTime = DateTime.Now,
//                    Subject = "Door1",
//                    DataVersion = "2.0"
//                });
//            }
//            return eventsList;
//        }
//    }
//}
