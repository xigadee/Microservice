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
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// This class is used to manage the TcpTls connection to the server.
    /// </summary>
    public class TcpTlsClient : TcpTlsBase
    {
        int mPollActive = 0;

        TcpTlsConnection mConnection;

        ConcurrentQueue<HttpProtocolRequestMessage> mOutgoing => new ConcurrentQueue<HttpProtocolRequestMessage>();

        ConcurrentQueue<TransmissionPayload> mIncoming => new ConcurrentQueue<TransmissionPayload>();

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="endPoint">The endpoint for the client to connect.</param>
        /// <param name="sslProtocolLevel">The TLS protocol level. Set this to None if you do not wish the connection to be encrypted.</param>
        /// <param name="serverCertificate">The server certificate to validate.</param>
        public TcpTlsClient(IPEndPoint endPoint, SslProtocols sslProtocolLevel, X509Certificate serverCertificate)
            : base(endPoint, sslProtocolLevel, serverCertificate)
        {

        }

        #region Start()
        /// <summary>
        /// This override is used to create the listening port.
        /// </summary>
        public override void Start()
        {
            mConnection = new TcpTlsConnection();
            mConnection.Client = new TcpClient();
            mConnection.Client.Connect(EndPoint);
            var stream = mConnection.Client.GetStream();

            if (SslProtocolLevel == SslProtocols.None)
                mConnection.DataStream = stream;
            else
            {
                mConnection.SslStream = new SslStream(stream, false, ValidateServerCertificate);

                // The server name must match the name on the server certificate.
                try
                {
                    mConnection.SslStream.AuthenticateAsClient(EndPoint.Address.ToString());
                }
                catch (AuthenticationException e)
                {
                    mConnection.Client.Close();
                    return;
                }

                mConnection.DataStream = mConnection.SslStream;
            }
        }
        #endregion
        #region Stop()
        /// <summary>
        /// This override is used to close the listening connections.
        /// </summary>
        public override void Stop()
        {

        }
        #endregion

        /// <summary>
        /// This method writes the incoming payload to a Http protocol message.
        /// </summary>
        /// <param name="payload">The incoming payload.</param>
        /// <returns>This method is async.</returns>
        public virtual Task Write(TransmissionPayload payload)
        {
            var message = new HttpProtocolRequestMessage();

            message.BeginInit();

            message.Instruction.Verb = "POST";
            message.Instruction.Instruction = @"/" + payload.Message.ToServiceMessageHeader().Key;

            message.HeaderAdd("HeaderId", payload.Id.ToString("N"));
            message.HeaderAdd("ProcessCorrelationKey", payload.Message.ProcessCorrelationKey);

            message.EndInit();

            //Add the message to the outgoing queue.
            mOutgoing.Enqueue(message);

            return Task.FromResult(0);
        }


        // The following method is invoked by the RemoteCertificateValidationDelegate.
        protected virtual bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        /// <summary>
        /// This method indicates whether the client requires attention.
        /// </summary>
        public override bool PollRequired => (mPollActive == 1) || (mConnection?.DataStream?.CanRead ?? false) || (mConnection?.DataStream?.CanWrite ?? false) || mOutgoing.Count>0;

        public override async Task Poll()
        {
            if (Interlocked.CompareExchange(ref mPollActive, 1, 0) == 1)
                return;

            try
            {
                while (mOutgoing.Count > 0)
                {
                    HttpProtocolRequestMessage message;
                    if (!mOutgoing.TryDequeue(out message))
                        break;

                    var buffer = message.ToArray();

                    await mConnection.DataStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                mPollActive = 0;
            }
        }
    }
}
