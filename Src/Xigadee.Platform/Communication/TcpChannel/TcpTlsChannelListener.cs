#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This channel uses the TCP and TLS protocol to communicate between Microservices.
    /// </summary>
    public class TcpTlsChannelListener : MessagingListenerBase<TcpTlsServerConnector, TcpTlsMessage, TcpTlsServerHolder>, IListenerPoll
    {
        #region EndPoint
        /// <summary>
        /// This is the endpoint that the connection should listen on.
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
        #endregion
        #region SslProtocolLevel
        /// <summary>
        /// This is the SslProtocol level. By default it is Tls12.
        /// </summary>
        public SslProtocols SslProtocolLevel { get; set; } = SslProtocols.Tls12;
        #endregion
        #region ServerCertificate
        /// <summary>
        /// This is the server certificate. This needs to be set if the ProtocolLevel is set to anything other than SslProtocols.None
        /// </summary>
        public X509Certificate ServerCertificate { get; set; }
        #endregion
        #region SettingsValidate()
        /// <summary>
        /// This method validates the settings necessary to start the sender.
        /// </summary>
        protected override void SettingsValidate()
        {
            base.SettingsValidate();

            if (EndPoint == null)
                throw new TcpTlsChannelConfigurationException($"{nameof(TcpTlsChannelListener)}: {ChannelId} Listening endpoint not set");

            if (SslProtocolLevel != SslProtocols.None && ServerCertificate == null)
                throw new TcpTlsChannelConfigurationException($"{nameof(TcpTlsChannelListener)}: {ChannelId} Certificate not set for {SslProtocolLevel}");
        }
        #endregion

        TcpListener mTcpListener = null;
        ConcurrentBag<TcpTlsClientWrapper> mTcpClients = null;

        /// <summary>
        /// This override is used to create the listening port.
        /// </summary>
        protected override void TearUp()
        {
            mTcpClients = new ConcurrentBag<TcpTlsClientWrapper>();
            mTcpListener = new TcpListener(EndPoint);
            mTcpListener.Start();
        }
        /// <summary>
        /// This override is used to close the listening connections.
        /// </summary>
        protected override void TearDown()
        {
            var activeClients = mTcpClients.ToList().Select((c) => ClientClose(c)).ToArray() ;
            Task.WaitAll(activeClients);

        }

        /// <summary>
        /// This override creates the client and registers/unregisters it with the protocol.
        /// </summary>
        /// <param name="partition">The partition to create the client for.</param>
        /// <returns>Returns the client holder.</returns>
        protected override TcpTlsServerHolder ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Type = "TcpTls Listener";
            client.Name = $"Channel{partition.Priority}";

            return client;
        }

        /// <summary>
        /// This property determines whether the listener requires a poll.
        /// </summary>
        public bool PollRequired
        {
            get
            {
                try
                {
                    if (mTcpListener?.Pending() ?? false
                        || mTcpClients.FirstOrDefault((c) => c.CanRead) != null
                        || mTcpClients.FirstOrDefault((c) => c.CanClose) != null
                        )
                        return true;

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This method returns a job that can be used to monitor communication.
        /// </summary>
        /// <returns>Async.</returns>
        public async Task Poll()
        {
            //Firstly, do we have any awaiting clients to process.
            if (Status == ServiceStatus.Running && (mTcpListener?.Pending() ?? false))
            {
                var client = await mTcpListener.AcceptTcpClientAsync();
                await ClientOpen(new TcpTlsClientWrapper { Client = client });
            }

            //Find the clients that are pending closure and cloe them.
            var close = mTcpClients
                .Where((c) => c.CanClose)
                .Select((c) => ClientClose(c));

            await Task.WhenAll(close);

            //Ok, do we have anything to read?
            var reads = mTcpClients
                .Where((c) => c.CanRead)
                .Select((c) => ClientRead(c));

            await Task.WhenAll(reads);
        }

        private async Task ClientOpen(TcpTlsClientWrapper client)
        {
            // A client has connected. Create the 
            // SslStream using the client's network stream.
            client.SslStream = new SslStream(client.Client.GetStream(), false);
            // Authenticate the server but don't require the client to authenticate.
            try
            {
                await client.SslStream.AuthenticateAsServerAsync(ServerCertificate, false, SslProtocolLevel, true);
                // Set timeouts for the read and write to 5 seconds.
                client.SslStream.ReadTimeout = 200;
                client.SslStream.WriteTimeout = 200;

                // Write a message to the client.
                byte[] message = Encoding.UTF8.GetBytes("Hello from the server.<EOF>");
                Console.WriteLine("Sending hello message.");
                client.SslStream.Write(message);

                mTcpClients.Add(client);
            }
            catch (AuthenticationException e)
            {
                client.SslStream.Close();
                client.Client.Close();
                return;
            }
        }

        private async Task ClientRead(TcpTlsClientWrapper client)
        {
            if (!client.SslStream.CanRead)
                return;

            //int length = await client.SslStream.ReadAsync();
        }

        private async Task ClientClose(TcpTlsClientWrapper client)
        {

        }

    }
}
