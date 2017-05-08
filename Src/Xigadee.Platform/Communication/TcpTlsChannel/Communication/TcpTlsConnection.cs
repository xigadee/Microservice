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
    /// This wrapper holds the connection.
    /// </summary>
    public class TcpTlsConnection
    {
        public byte[] Buffer => new byte[10240];

        public Guid Id => Guid.NewGuid();

        public TcpClient Client { get; set; }

        public SslStream SslStream { get; set; }

        public bool CanClose
        {
            get
            {
                try
                {
                    return !Client.Connected;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool CanRead
        {
            get
            {
                try
                {
                    return SslStream?.CanRead ?? false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
