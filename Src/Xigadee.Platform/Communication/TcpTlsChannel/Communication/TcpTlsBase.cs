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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for TcpTls functionality.
    /// </summary>
    public abstract class TcpTlsBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="sslProtocolLevel"></param>
        /// <param name="serverCertificate"></param>
        protected TcpTlsBase(IPEndPoint endPoint, SslProtocols sslProtocolLevel, X509Certificate serverCertificate)
        {
            EndPoint = endPoint;
            SslProtocolLevel = sslProtocolLevel;
            ServerCertificate = serverCertificate;
        }
        #endregion

        #region EndPoint
        /// <summary>
        /// This is the endpoint that the connection should listen on.
        /// </summary>
        public IPEndPoint EndPoint { get; }
        #endregion
        #region SslProtocolLevel
        /// <summary>
        /// This is the SslProtocol level. By default it is Tls12.
        /// </summary>
        public SslProtocols SslProtocolLevel { get; }
        #endregion
        #region ServerCertificate
        /// <summary>
        /// This is the server certificate. This needs to be set if the ProtocolLevel is set to anything other than SslProtocols.None
        /// </summary>
        public X509Certificate ServerCertificate { get; }
        #endregion

        #region Start()
        /// <summary>
        /// This abstract method is used to start.
        /// </summary>
        public abstract void Start();

        #endregion
        #region Stop()
        /// <summary>
        /// This abstract method is used to stop.
        /// </summary>
        public abstract void Stop();
        #endregion
    }
}
