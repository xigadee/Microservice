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

namespace Xigadee
{
    /// <summary>
    /// This class holds an incoming or outgoing connection.
    /// </summary>
    public class TcpTlsConnection
    {
        protected readonly ConcurrentQueue<TransmissionPayload> mQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EndPoint"></param>
        /// <param name="protocolLevel"></param>
        /// <param name="serverCertificate"></param>
        public TcpTlsConnection(IPEndPoint EndPoint, SslProtocols protocolLevel = SslProtocols.Tls12, X509Certificate serverCertificate = null)
        {
            this.ProtocolLevel = ProtocolLevel;
            this.ServerCertificate = serverCertificate;
            this.EndPoint = EndPoint;
            mQueue = new ConcurrentQueue<TransmissionPayload>();
        }

        public X509Certificate ServerCertificate { get; }

        public IPEndPoint EndPoint { get; }
        /// <summary>
        /// This property specifies whether the connection should use Tls to secure the connection.
        /// </summary>
        public SslProtocols ProtocolLevel { get; }

        public TcpTlsConnection Register(TcpTlsClientHolder client)
        {
            return this;
        }

        public TcpTlsConnection UnRegister(TcpTlsClientHolder client)
        {
            return this;
        }

        public void Close() { }

        public void Send()
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
