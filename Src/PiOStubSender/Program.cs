using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PiOStubSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var ep = new IPEndPoint(IPAddress.Loopback, 9761);
            UdpClient uc = new UdpClient(9760);
            Console.WriteLine("Press Enter to send.");
            while (true)
            {
                Console.ReadLine();
                //for (int i = 0; i < 11; i++)
                //{
                    byte[] data = Encoding.UTF8.GetBytes($"{{Hello mom = '{Environment.TickCount}'}}");
                    uc.Send(data, data.Length, ep);
                //}

                Console.WriteLine("Sent. Press Enter to send again.");
            }
        }
    }
}
