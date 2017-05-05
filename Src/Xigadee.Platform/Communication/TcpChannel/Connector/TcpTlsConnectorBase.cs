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
using System.Linq;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Xigadee;

namespace Xigadee
{

    /// <summary>
    /// This class holds an incoming or outgoing connection.
    /// </summary>
    public abstract class TcpTlsConnectorBase: ServiceBase<StatusBase>
    {
        /// <summary>
        /// This is the queue that contains 
        /// </summary>
        protected readonly ConcurrentQueue<TransmissionPayload> mQueue;
        /// <summary>
        /// This is the default constructor for the endpoints.
        /// </summary>
        /// <param name="type">This is the connection type.</param>
        /// <param name="endPoint">The endpoint.</param>
        /// <param name="protocolLevel">The protocol level. Please note that SslProtocols.None will result in unencrypted transmission.</param>
        /// <param name="serverCertificate">The SSL certificate, if required.</param>
        public TcpTlsConnectorBase(IPEndPoint endPoint, SslProtocols protocolLevel = SslProtocols.Tls12, X509Certificate serverCertificate = null)
        {
            ProtocolLevel = protocolLevel;
            ServerCertificate = serverCertificate;
            EndPoint = endPoint;
            mQueue = new ConcurrentQueue<TransmissionPayload>();

            if (EndPoint == null)
                throw new ArgumentNullException("endPoint", "Endpoint cannot be null.");
        }
        /// <summary>
        /// This is the SSL security certificate.
        /// </summary>
        public X509Certificate ServerCertificate { get; }
        /// <summary>
        /// This is the IPEndpoint for either listening on, or connecting to.
        /// </summary>
        public IPEndPoint EndPoint { get; }
        /// <summary>
        /// This property specifies whether the connection should use Tls to secure the connection.
        /// By default, this will be set to Tls12.
        /// </summary>
        public SslProtocols ProtocolLevel { get; }





        public void Close() { }

        public void Send(TransmissionPayload payload)
        {
            HttpRequestMessage rq = new HttpRequestMessage(HttpMethod.Get, "https://hello.com");

            HttpClientHandler hn = new HttpClientHandler();
            

            
            //HttpListenerContext cx;
            //cx.
            //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            //requestMessage.Headers.ExpectContinue = false;

            //MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            //ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
            //byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            //multiPartContent.Add(byteArrayContent, "this is the name of the content", fileName);
            //requestMessage.Content = multiPartContent;

            //DelegatingHandler h;
            //HttpListener listen = new HttpListener();
            //listen.
            //listen.Start();

            //HttpClient httpClient = new HttpClient();
            //try
            //{
            //    Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
            //    HttpResponseMessage httpResponse = httpRequest.Result;
            //    HttpStatusCode statusCode = httpResponse.StatusCode;
            //    HttpContent responseContent = httpResponse.Content;

            //    if (responseContent != null)
            //    {
            //        Task<String> stringContentsTask = responseContent.ReadAsStringAsync();
            //        String stringContents = stringContentsTask.Result;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

        }

    }

}
