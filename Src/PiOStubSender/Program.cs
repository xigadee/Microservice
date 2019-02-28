using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using PiO;

namespace PiOStubSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var name = Dns.GetHostName();
            var host = Dns.GetHostEntry(name);

            var ipList = host.AddressList.Where((ip) => ip.AddressFamily == AddressFamily.InterNetwork);

            var ep = new IPEndPoint(ipList.First(), 9761);
            UdpClient uc = new UdpClient(9760);
            Console.WriteLine("Press Enter to send.");
            int id = 0;
            while (true)
            {
                Console.ReadLine();
                //for (int i = 0; i < 11; i++)
                //{

                //var message = new LightwaveMessage();
                dynamic message = new ExpandoObject();
                message.trans = Environment.TickCount;
                message.Title = "Mr.";
                message.Name = "Sid";
                message.City = "London";
                message.TimeStamp = DateTime.UtcNow;
                message.Id = id;
                //message.Trans = Environment.TickCount;

                var authorData = JsonConvert.SerializeObject(message, Formatting.None);

                byte[] data = Encoding.UTF8.GetBytes(authorData);
                byte[] dataEx = new byte[data.Length + 2];
                Array.ConstrainedCopy(data, 0, dataEx, 2, data.Length);
                //byte[] data = new byte[65507];
                uc.Send(dataEx, dataEx.Length, ep);
                //}

                Console.WriteLine("Sent. Press Enter to send again.");
                id++;
            }
        }
    }
}
