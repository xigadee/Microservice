using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xigadee;

namespace PiO
{
    class Program
    {
        static MicroservicePipeline mservice;

        static void UdpTransmitHelper(string mcastGroup, int port, int ttl, string rep)
        {
            IPAddress ip;
            try
            {
                Console.WriteLine("MCAST Send on Group: {0} Port: {1} TTL: {2}", mcastGroup, port, ttl);
                ip = IPAddress.Parse(mcastGroup);

                Socket s = new Socket(AddressFamily.InterNetwork,
                                SocketType.Dgram, ProtocolType.Udp);

                s.SetSocketOption(SocketOptionLevel.IP,
                    SocketOptionName.AddMembership, new MulticastOption(ip));

                s.SetSocketOption(SocketOptionLevel.IP,
                    SocketOptionName.MulticastTimeToLive, ttl);

                byte[] b = new byte[10];
                for (int x = 0; x < b.Length; x++) b[x] = (byte)(x + 65);

                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(mcastGroup), port);

                Console.WriteLine("Connecting...");

                s.Connect(ipep);

                for (int x = 0; x < int.Parse(rep); x++)
                {
                    Console.WriteLine("Sending ABCDEFGHIJ...");
                    s.Send(b, b.Length, SocketFlags.None);
                }

                Console.WriteLine("Closing Connection...");
                s.Close();
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        static void UdpReceiveHelper(string mcastGroup, int port)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            s.Bind(ipep);

            IPAddress ip = IPAddress.Parse(mcastGroup);

            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

            while (true)
            {
                byte[] b = new byte[10];
                Console.WriteLine("Waiting for data..");
                s.Receive(b);
                string str = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);
                Console.WriteLine("RX: " + str.Trim());
            }
            //s.Close();
        }

        static void Main(string[] args)
        {
            var name = Dns.GetHostName();
            var host = Dns.GetHostEntry(name);
            var listv4 = host.AddressList.Where((ip) => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
            var listv6 = host.AddressList.Where((ip) => ip.AddressFamily == AddressFamily.InterNetworkV6).ToList();


            DebugMemoryDataCollector coll;
            mservice = new MicroservicePipeline("PiO", description: "PiO Server");
            mservice
                .AdjustPolicyTaskManagerForDebug()
                .ConfigurationSetFromConsoleArgs(args)
                .AddDebugMemoryDataCollector(out coll)
                .AddChannelIncoming("lightwave", "LightwaveRF UDP traffic", ListenerPartitionConfig.Init(1))
                    .AttachUdpListener(new IPEndPoint(IPAddress.Any, 9761)
                        , requestAddress: ("message","in")
                        , deserialize: (holder) => holder.SetObject(new LightwaveMessage(holder.Blob, (IPEndPoint)holder.Metadata)))
                    .AttachCommand(async (ctx) => 
                    {
                        //Do nothing
                        await Task.Delay(10);
                    }
                    , ("message", "in"))
                    .Revert()
                .AddChannelOutgoing("status", "Outgoing UDP status", SenderPartitionConfig.Init(1))
                    .AttachUdpSender(new IPEndPoint(IPAddress.Parse("255.255.255.255"), 44723)
                        , serializer: new StatisticsSummaryLogUdpSerializer())
                    .Revert()
                .OnDataCollection
                (
                    (ctx,ev) => 
                    {
                        ctx.Outgoing.Process(("status", null, null), ev.Data, 1, ProcessOptions.RouteExternal);
                    }
                    , DataCollectionSupport.Statistics
                )
                ;

            mservice.StartWithConsole();
        }
    }
}
