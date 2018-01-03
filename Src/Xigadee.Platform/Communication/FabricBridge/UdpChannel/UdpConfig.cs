using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the mode for the UDPHelper class.
    /// </summary>
    public enum UdpMode
    {
        /// <summary>
        /// UDP is in unicast mode.
        /// </summary>
        Unicast,
        /// <summary>
        /// UDP is in broadcast mode.
        /// </summary>
        Broadcast,
        /// <summary>
        /// UDP is in multicast mode.
        /// </summary>
        Multicast
    }

    /// <summary>
    /// This class holds the various UDP configuration modes and provides shortcut constructors to the supported modes.
    /// </summary>
    public class UdpConfig
    {
        private UdpConfig() { }

        public static UdpConfig UnicastIpEndPoint(IPEndPoint ep)
        {
            return new UdpConfig { Port = ep.Port, Addresses = new IPAddress[] { ep.Address }, Mode = UdpMode.Unicast };
        }

        public static UdpConfig UnicastAllIps(int port)
        {
            return new UdpConfig { Port = port, Addresses = IpAddresses().ToList(), Mode = UdpMode.Unicast };
        }

        public static UdpConfig BroadcastAllIps(int port)
        {
            return new UdpConfig { Port = port, Addresses = IpAddresses().ToList(), Mode = UdpMode.Broadcast };
        }


        /// <summary>
        /// Gets the multicast time to live.
        /// </summary>
        public int? MulticastTtl { get; private set;}

        /// <summary>
        /// Gets the port that the Udp transport is using.
        /// </summary>
        public int Port { get; private set;}
        /// <summary>
        /// Gets the Udp mode.
        /// </summary>
        public UdpMode Mode { get; private set;}
        /// <summary>
        /// Gets the IP addresses that the Udp transport is active on.
        /// </summary>
        public IEnumerable<IPAddress> Addresses { get; private set;}

        #region Static -> IpAddresses(AddressFamily family = AddressFamily.InterNetwork)
        /// <summary>
        /// This static method resolves the IP addresses for the machine.
        /// </summary>
        /// <param name="family">The address family. The default is IPV4.</param>
        /// <returns>Returns a list of IP address of the specified type for this machine.</returns>
        public static IEnumerable<IPAddress> IpAddresses(AddressFamily family = AddressFamily.InterNetwork)
        {
            var name = Dns.GetHostName();
            var host = Dns.GetHostEntry(name);

            return host.AddressList.Where((ip) => ip.AddressFamily == family);
        }
        #endregion
    }
}
