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
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This sender uses the TCP and TLS protocols to efficently transmit requests to a remote Microservices.
    /// </summary>
    public class TcpTlsChannelSender :MessagingSenderBase<TcpTlsConnection, TcpTlsMessage, TcpTlsClientHolder>
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
        #region SettingsValidate()
        /// <summary>
        /// This method validates the settings necessary to start the sender.
        /// </summary>
        protected override void SettingsValidate()
        {
            base.SettingsValidate();

            if (EndPoint == null)
                throw new TcpTlsChannelConfigurationException($"{nameof(TcpTlsChannelSender)}: {ChannelId}");
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        protected override TcpTlsClientHolder ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Type = "TcpTls Sender";
            client.Name = partition.Priority.ToString();

            return client;
        }
    }
}
