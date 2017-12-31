using System;
using System.Dynamic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace PiOStubSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var ep = new IPEndPoint(IPAddress.Loopback, 9761);
            UdpClient uc = new UdpClient(9760);
            Console.WriteLine("Press Enter to send.");
            int id = 0;
            while (true)
            {
                Console.ReadLine();
                //for (int i = 0; i < 11; i++)
                //{

                dynamic message = new ExpandoObject();
                message.Title = "Mr.";
                message.Name = "Sid";
                message.City = "London";
                message.TimeStamp = Environment.TickCount;
                message.Id = id;

                var authorData = JsonConvert.SerializeObject(message, Formatting.Indented);

                byte[] data = Encoding.UTF8.GetBytes(authorData);
                uc.Send(data, data.Length, ep);
                //}

                Console.WriteLine("Sent. Press Enter to send again.");
                id++;
            }
        }
    }
}
