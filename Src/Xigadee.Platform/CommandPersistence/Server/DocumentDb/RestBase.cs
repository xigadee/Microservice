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

#region using
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
#endregion
namespace Xigadee
{
    //  -Database Account
    //      -Database
    //          -Collection
    //              -Document
    //                  -Attachment
    //              -Stored Procedure
    //              -Trigger
    //              -User-defined functions
    //          -User
    //              -Permission
    //      -Media

    /// <summary>
    /// This is the REST base class for calling DocumentDb.
    /// </summary>
    public abstract class RestBase
    {
        #region Declarations
        public const string sBase = "https://{0}.documents.azure.com";

        public const string sDB = "/dbs/{0}";

        public const string sDBUser = "/users/{0}";
        public const string sDBUserPermission = "/permissions/{0}";

        public const string sDBCollection = "/colls/{0}";

        public const string sDBCollectionStoredProcedure = "/sprocs/{0}";
        public const string sDBCollectionTrigger = "/triggers/{0}";
        public const string sDBCollectionUDF = "/udfs/{0}";
        public const string sDBCollectionDocument = "/docs/{0}";

        public const string sDBCollectionDocumentAttachment = "/attachments/{0}";

        /// <summary>
        /// This is the default timeout for requests.
        /// </summary>
        protected TimeSpan? mDefaultTimeout;
        #endregion

        protected RestBase(DocumentDbConnection connection, TimeSpan? defaultTimeout = null)
        {
            mDefaultTimeout = defaultTimeout;
            Connection = connection;
        }

        public DocumentDbConnection Connection { get; protected set; }

        public string Name { get; set; }

        protected HttpRequestMessage Request(HttpMethod verb, string resourceType, string resourceId, string uriPart
            , string xmsdate = null
            , string date = null
            , string eTag = null
            , bool idBased = false)
        {
            if (xmsdate == null)
                xmsdate = DateTime.UtcNow.ToString("r");
            string auth = CalculateAuth(verb.Method.ToUpper(), resourceType, resourceId, xmsdate, idBased:idBased);

            HttpRequestMessage rq = new HttpRequestMessage();
            rq.Method = verb;
            rq.RequestUri = BuildUri(uriPart);
            rq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (eTag != null)
                rq.Headers.Add("If-Match", eTag);

            rq.Headers.Add("x-ms-date", xmsdate);
            rq.Headers.Add("x-ms-version", "2015-08-06");
            //From 2014-08-21 to 2015-08-06 - https://msdn.microsoft.com/en-us/library/azure/dn781481.aspx
            rq.Headers.Authorization = new AuthenticationHeaderValue(auth);

            return rq;
        }

        protected string CalculateAuth(string verb, string resourceType, string resourceId, string xmsdate, string date = "", bool idBased = false)
        {
            string sig = string.Format(CultureInfo.InvariantCulture,
                "{0}\n{1}\n{2}\n{3}\n{4}\n"
                , verb.ToLowerInvariant()
                , resourceType.ToLowerInvariant()
                , idBased?resourceId: resourceId.ToLowerInvariant()
                , xmsdate.ToLowerInvariant()
                , date.ToLowerInvariant());

            //string sig = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n",
            //    verb, resourceType, resourceId, xmsdate, date).ToLowerInvariant();

            byte[] hash = Encoding.UTF8.GetBytes(sig);

            string signature;
            using (var hashstring = new HMACSHA256(Connection.Key))
            {
                byte[] sha256Hash = hashstring.ComputeHash(hash);

                signature = Convert.ToBase64String(sha256Hash);
            }

            string auth = string.Format("type=master&ver=1.0&sig={0}", signature);

            return System.Net.WebUtility.UrlEncode(auth);
        }

        protected Uri BuildUri(string uriPart)
        {
            return new Uri(Connection.Account, uriPart);
        }

        protected async Task<ResponseHolder> CallClient(HttpMethod verb, string resourceType, string resourceId, string uriPart
            , string xmsdate = null
            , string date = null
            , HttpContent content = null
            , Action<HttpRequestMessage> adjust = null
            , CancellationToken? cancel = null
            , TimeSpan? timeout = null
            , string eTag = null
            , bool waitOnThrottle = true
            , bool idBased = false)
        {
            return await CallClient<ResponseHolder>(verb, resourceType, resourceId, uriPart
                , xmsdate: xmsdate
                , date: date
                , content: content
                , adjust: adjust
                , cancel: cancel
                , timeout: timeout
                , eTag: eTag
                , idBased: idBased);
        }

        protected async Task<R> CallClient<R>(HttpMethod verb, string resourceType, string resourceId, string uriPart
            , string xmsdate = null
            , string date = null
            , HttpContent content = null
            , Action<HttpRequestMessage> adjust = null
            , Action<R> mapper = null
            , CancellationToken? cancel = null
            , TimeSpan? timeout = null
            , string eTag = null
            , bool autoWaitOnThrottle = true
            , bool idBased = false)
            where R: ResponseHolder, new()
        {
            var rs = new R();

            try
            {
                using (HttpRequestMessage rq = Request(verb, resourceType, resourceId, uriPart, eTag: eTag, idBased: idBased))
                using (var client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

                    if (adjust != null)
                        adjust(rq);
                    if (content != null)
                        rq.Content = content;
                    
                    if ((timeout ?? mDefaultTimeout).HasValue)
                        client.Timeout = (timeout ?? mDefaultTimeout).Value;

                    // Specify request body
                    if (cancel.HasValue)
                        rs.Response = await client.SendAsync(rq, cancel.Value);
                    else
                        rs.Response = await client.SendAsync(rq);

                    if (rs.Response.Content != null)
                    {
                        rs.Content = await rs.Response.Content.ReadAsStringAsync();

                        if (idBased)
                        {
                            var jObj = JObject.Parse(rs.Content);
                            var token = jObj.GetValue("_rid");
                            if (token != null)
                                rs.DocumentId = token.Value<string>();
                        }
                    }

                    rs.IsSuccess = rs.Response.IsSuccessStatusCode;
                    rs.IsThrottled = (int)rs.Response.StatusCode == 429;
                    rs.IsTimeout = rs.IsThrottled || rs.Response.StatusCode == HttpStatusCode.RequestTimeout;

                    //OK, we need to get the throttle wait time.
                    int throttleWaitTime;
                    if (rs.IsThrottled
                        && rs.Response.Headers.Contains("x-ms-retry-after-ms")
                        && int.TryParse(rs.Response.Headers.GetValues("x-ms-retry-after-ms").FirstOrDefault(), out throttleWaitTime))
                    {
                        rs.ThrottleSuggestedWait = TimeSpan.FromMilliseconds(throttleWaitTime);
                    }

                    if (rs.Response.Headers.Contains("x-ms-request-charge"))
                    {
                        double charge;
                        if (double.TryParse(rs.Response.Headers.GetValues("x-ms-request-charge").FirstOrDefault(), out charge))
                            rs.ResourceCharge = charge;
                    }

                    //x-ms-request-charge 
                    //Ok, we will if the autowait is true and the throttle time has been set
                    if (autoWaitOnThrottle && rs.IsThrottled && rs.ThrottleSuggestedWait.HasValue)
                    {
                        await Task.Delay(rs.ThrottleSuggestedWait.Value);
                        rs.ThrottleHasWaited = true;
                    }

                    if (rs.Response.Headers != null)
                    {
                        if (rs.Response.Headers.ETag != null)
                            rs.ETag = rs.Response.Headers.ETag.Tag;

                        if (rs.Response.Headers.Contains("x-ms-session-token"))
                            rs.SessionToken = rs.Response.Headers.GetValues("x-ms-session-token").FirstOrDefault();

                        if (rs.Response.Headers.Contains("x-ms-continuation"))
                            rs.ContinuationToken = rs.Response.Headers.GetValues("x-ms-continuation").FirstOrDefault();
                    }
                }

                if (mapper != null)
                    mapper(rs);
            }
            catch (TaskCanceledException tcex)
            {
                rs.Ex = tcex;
                rs.IsTimeout = true;
                rs.IsSuccess = false;
            }
            catch (Exception ex)
            {
                rs.Ex = ex;
                rs.IsSuccess = false;
            }

            return rs;
        }
    }
}
