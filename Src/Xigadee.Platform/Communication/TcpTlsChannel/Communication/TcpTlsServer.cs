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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the server holder.
    /// </summary>
    public class TcpTlsServer: TcpTlsBase
    {
        #region Declarations
        /// <summary>
        /// This is the listener.
        /// </summary>
        protected TcpListener mTcpListener = null;
        /// <summary>
        /// This is the connected client connection.
        /// </summary>
        protected ConcurrentBag<TcpTlsConnection> mTcpClients = null;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="endPoint">This is the listening endpoint.</param>
        /// <param name="sslProtocolLevel">This is the SSL protocol level. Set this to None to disable encryption.</param>
        /// <param name="serverCertificate">This is the optional server certificate. This must be set if ssl encryption is set.</param>
        public TcpTlsServer(IPEndPoint endPoint, SslProtocols sslProtocolLevel, X509Certificate serverCertificate):base(endPoint, sslProtocolLevel, serverCertificate)
        {
        } 
        #endregion

        #region Start()
        /// <summary>
        /// This override is used to create the listening port.
        /// </summary>
        public override void Start()
        {
            mTcpClients = new ConcurrentBag<TcpTlsConnection>();
            mTcpListener = new TcpListener(EndPoint);
            mTcpListener.Start();
        }
        #endregion
        #region Stop()
        /// <summary>
        /// This override is used to close the listening connections.
        /// </summary>
        public override void Stop()
        {
            mTcpListener.Stop();
            //var activeClients = mTcpClients.ToList().Select((c) => ClientClose(c)).ToArray();
            //Task.WaitAll(activeClients);
        }
        #endregion

        #region PollRequired
        /// <summary>
        /// This property determines whether the listener requires a poll.
        /// </summary>
        public override bool PollRequired
        {
            get
            {
                try
                {
                    if (mPollActive==0 &&(mTcpListener?.Pending() ?? false
                        || mTcpClients.FirstOrDefault((c) => c.CanRead) != null
                        || mTcpClients.FirstOrDefault((c) => c.CanClose) != null
                        ))
                        return true;

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        #endregion
        #region Poll()
        int mPollActive = 0;
        /// <summary>
        /// This method returns a job that can be used to monitor communication.
        /// </summary>
        /// <returns>Async.</returns>
        public override async Task Poll()
        {
            if (Interlocked.CompareExchange(ref mPollActive, 1, 0)==1)
                return;

            try
            {
                //Firstly, do we have any awaiting clients to process.
                if ((mTcpListener?.Pending() ?? false))
                {
                    var client = await mTcpListener.AcceptTcpClientAsync();
                    await ClientOpen(new TcpTlsConnection { Client = client });
                }

                //Find the clients that are pending closure and close them.
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
            catch (Exception ex)
            {

            }
            finally
            {
                mPollActive = 0;
            }

        }
        #endregion

        private async Task ClientOpen(TcpTlsConnection client)
        {
            if (SslProtocolLevel == SslProtocols.None)
            {
                client.DataStream = client.Client.GetStream();
            }
            else
            {
                // A client has connected. Create the 
                // SslStream using the client's network stream.
                client.SslStream = new SslStream(client.Client.GetStream());

                // Authenticate the server but don't require the client to authenticate.
                try
                {
                    await client.SslStream.AuthenticateAsServerAsync(ServerCertificate, false, SslProtocolLevel, true);
                    // Set timeouts for the read and write to 5 seconds.
                    client.SslStream.ReadTimeout = 200;
                    client.SslStream.WriteTimeout = 200;
                    client.DataStream = client.SslStream;
                }
                catch (AuthenticationException e)
                {
                    client.SslStream.Close();
                    client.Client.Close();
                    return;
                }
            }

            mTcpClients.Add(client);

            // Write a message to the client.
            byte[] message = Encoding.UTF8.GetBytes("Hello from the server.<EOF>");
            client.DataStream.Write(message, 0, message.Length);
            client.DataStream.Flush();
        }

        private async Task ClientRead(TcpTlsConnection client)
        {
            if (!client.SslStream.CanRead)
                return;

            int length = await client.DataStream.ReadAsync(client.Buffer,0, client.Buffer.Length);
        }

        private async Task ClientClose(TcpTlsConnection client)
        {
            client.DataStream.Flush();
            client.DataStream.Close();
        }

    }
}
