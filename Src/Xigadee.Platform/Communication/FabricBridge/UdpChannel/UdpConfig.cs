using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class holds the various UDP configuration modes and provides shortcut constructors to the supported modes.
    /// </summary>
    public class UdpConfig
    {
        private UdpConfig() { }

        public static UdpConfig UnicastIpEndPoint(IPEndPoint ep, IPEndPoint remoteEp = null)
        {
            return new UdpConfig
            {
                Port = ep.Port
                , Addresses = new IPAddress[] { ep.Address }
                , Mode = UdpMode.Unicast
                , RemoteEndPoint = remoteEp
            };
        }

        public static UdpConfig UnicastAllIps(int port)
        {
            return new UdpConfig
            {
                  Port = port
                , Addresses = IpAddresses().ToList()
                , Mode = UdpMode.Unicast
                , RemoteEndPoint = null
            };
        }

        public static UdpConfig UnicastAllIps(int port, string remoteHost, int? remotePort = null)
        {
            var host = Dns.GetHostEntry(remoteHost);

            return new UdpConfig
            {
                  Port = port
                , Addresses = IpAddresses().ToList()
                , Mode = UdpMode.Unicast
                , RemoteEndPoint = new IPEndPoint(host.AddressList.First(), remotePort ?? port)
            };
        }

        public static UdpConfig UnicastAllIps(int port, IPEndPoint remoteEp)
        {
            return new UdpConfig
            {
                  Port = port
                , Addresses = IpAddresses().ToList()
                , Mode = UdpMode.Unicast
                , RemoteEndPoint = remoteEp
            };
        }

        public static UdpConfig BroadcastAllIps(int port, int? destinationPort = null)
        {
            return new UdpConfig
            {
                  Port = port
                , Addresses = IpAddresses().ToList()
                , Mode = UdpMode.Broadcast
                , RemoteEndPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), destinationPort ?? port)
            };
        }

        public IPEndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// Gets the multicast time to live.
        /// </summary>
        public int? MulticastTtl { get; private set;}
        /// <summary>
        /// Gets or sets a value indicating whether to the UDP port exclusively. The default is false.
        /// </summary>
        public bool ExclusiveAddressUse { get; set; }= false;
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
}
