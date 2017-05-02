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

namespace Xigadee
{
    /// <summary>
    /// This channel uses the TCP and TLS protocol to communicate between Microservices.
    /// </summary>
    public class TcpTlsChannelListener : MessagingListenerBase<TcpTlsConnection, TcpTlsMessage, TcpTlsClientHolder>
    {
        #region EndPoint
        /// <summary>
        /// This is the endpoint that the connection should listen on.
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
        #endregion
        #region ProtocolLevel
        /// <summary>
        /// This is the SslProtocol level. By default it is Tls12.
        /// </summary>
        public SslProtocols ProtocolLevel { get; set; } = SslProtocols.Tls12;
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

            if (ProtocolLevel != SslProtocols.None && ServerCertificate == null)
                throw new TcpTlsChannelConfigurationException($"{nameof(TcpTlsChannelListener)}: {ChannelId} Certificate not set for {ProtocolLevel}");
        }
        #endregion

        /// <summary>
        /// This is the Protocol server.
        /// </summary>
        TcpTlsConnection mServer;

        /// <summary>
        /// This override is used to create the listening port.
        /// </summary>
        protected override void TearUp()
        {
            mServer = new TcpTlsConnection(EndPoint, ProtocolLevel, ServerCertificate);
        }
        /// <summary>
        /// This override is used to close the listening connections.
        /// </summary>
        protected override void TearDown()
        {
            mServer.Close();
            mServer = null;
        }

        /// <summary>
        /// This override creates the client and registers/unregisters it with the protocol.
        /// </summary>
        /// <param name="partition">The partition to create the client for.</param>
        /// <returns>Returns the client holder.</returns>
        protected override TcpTlsClientHolder ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Type = "TcpTls Listener";
            client.Name = $"Channel{partition.Priority}";

            client.ClientCreate = () => mServer.Register(client);

            client.ClientClose = () => mServer.UnRegister(client);

            return client;
        }
    }
}
