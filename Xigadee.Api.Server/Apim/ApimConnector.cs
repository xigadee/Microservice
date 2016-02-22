#region using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to retrieve the necessary details from the API Management interface.
    /// </summary>
    public class ApimConnector
    {
        #region Static -> CreateSharedAccessToken(string id, string key, TimeSpan? expiresIn = null)
        // expiry - the expiration date and time for the generated access token. 
        static private string CreateSharedAccessToken(string id, string key, TimeSpan? expiresIn = null)
        {
            var expiry = DateTime.UtcNow.Add(expiresIn ?? TimeSpan.FromDays(1));

            using(var encoder = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                string dataToSign = id + "\n" + expiry.ToString("O", CultureInfo.InvariantCulture);
                //string x = string.Format("{0}\n{1}", id, expiry.ToString("O", CultureInfo.InvariantCulture));
                var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                var signature = Convert.ToBase64String(hash);
                string encodedToken = string.Format("uid={0}&ex={1:o}&sn={2}", id, expiry, signature);
                return encodedToken;
            }
        } 
        #endregion

        #region Declarations
        private readonly Uri mApiUri;
        private string mToken;

        private readonly string mId;
        private readonly string mKey;
        private readonly TimeSpan? mTokenExpiry;

        private readonly string mApiVersion;

        private readonly ConcurrentDictionary<string, ApimPrincipal> mPrincipals;
        #endregion

        public ApimConnector(string id, string key, Uri apiUri, TimeSpan? tokenExpiry = null, string apiVersion = "2014-02-14-preview")
            :this(apiUri, CreateSharedAccessToken(id, key, tokenExpiry), apiVersion)
        {
            if(string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException();

            mId = id;
            mKey = key;
            mTokenExpiry = tokenExpiry;
        }

        public ApimConnector(Uri apiUri, string token, string apiVersion="2014-02-14-preview")
        {
            if(apiUri == null)
                throw new ArgumentNullException("apiUri");

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            mApiUri = apiUri;
            mPrincipals = new ConcurrentDictionary<string, ApimPrincipal>();

            if(token.StartsWith("SharedAccessSignature ", StringComparison.InvariantCultureIgnoreCase))
                token = token.Substring(22);

            mToken = token;
            mApiVersion = apiVersion;
            mId = null;
            mKey = null;
        }

        public void Load()
        {
            var subsResponse = CallClient(HttpMethod.Get, @"/subscriptions").Result;
            if (!subsResponse.IsSuccess)
            {
                if (subsResponse.Response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AuthenticationException("Unable to authenticate to APIM");

                throw new InvalidOperationException("Unable to load APIM Subscriptions", subsResponse.Ex);
            }

            var usersResponse = CallClient(HttpMethod.Get, @"/users").Result;
            if (!usersResponse.IsSuccess)
                throw new InvalidOperationException("Unable to load APIM Users", usersResponse.Ex);

            var subscriptions = JsonConvert.DeserializeObject<ApimSubscriptions>(subsResponse.Content).ApimSubscriptionDetails;
            var users = JsonConvert.DeserializeObject<ApimUsers>(usersResponse.Content)
                .ApimUserDetails.ToDictionary(ud => ud.id, ud => ud);

            // Remove subsriptions that no longer appear in the list
            ApimPrincipal removedPrincipal;
            mPrincipals.Keys.Except(subscriptions.SelectMany(s => new List<string> {s.primaryKey, s.secondaryKey}))
                .ToList().ForEach(rk => mPrincipals.TryRemove(rk, out removedPrincipal));

            foreach (var subscription in subscriptions)
            {
                ApimUserDetail user;
                if (!users.TryGetValue(subscription.userId, out user))
                    continue;

                var principal = CreatePrincipal(user);
                mPrincipals.AddOrUpdate(subscription.primaryKey, principal, (s, ap) => principal);
                mPrincipals.AddOrUpdate(subscription.secondaryKey, principal, (s, ap) => principal);
            }
        }

        public void Refresh()
        {
            try
            {
                Load();
            }
            catch (AuthenticationException)
            {
                // Refresh token as it looks like it has expired
                RefreshToken(mTokenExpiry);
            }
            catch (Exception)
            {
                // Assume some kind of intermittent fault which will be resolved on next refresh
            }
        }

        public bool TryGetValue(string subscriptionId, out ApimPrincipal principal)
        {
            return mPrincipals.TryGetValue(subscriptionId, out principal);
        }

        public void RefreshToken(TimeSpan? tokenExpiry = null)
        {
            if(string.IsNullOrWhiteSpace(mId) || string.IsNullOrWhiteSpace(mKey))
                throw new ArgumentNullException();

            mToken = CreateSharedAccessToken(mId, mKey, tokenExpiry);
        }

        protected HttpRequestMessage Request(HttpMethod verb, string uriPart, string query)
        {
            var builder = new UriBuilder(mApiUri)
            {
                Path = uriPart,
                Query = query == null
                    ? string.Concat("api-version=", mApiVersion)
                    : string.Concat("api-version=", mApiVersion, "&", query)
            };

            var rq = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = builder.Uri
            };
            rq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            rq.Headers.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", mToken);
            return rq;
        }


        public async Task<ApimResponseHolder> CallClient(HttpMethod verb, string uriPart
            , string query = null
            , HttpContent content = null
            , Action<HttpRequestMessage> adjust = null
            , Action<ApimResponseHolder> mapper = null
            , CancellationToken? cancel = null
            , TimeSpan? timeout = null)
        {
            var rs = new ApimResponseHolder();

            try
            {
                HttpRequestMessage rq = Request(verb, uriPart, query);

                if(adjust != null)
                    adjust(rq);
                if(content != null)
                    rq.Content = content;

                var client = new HttpClient();
                if(timeout.HasValue)
                    client.Timeout = timeout.Value;

                // Specify request body
                if(cancel.HasValue)
                    rs.Response = await client.SendAsync(rq, cancel.Value);
                else
                    rs.Response = await client.SendAsync(rq);

                if(rs.Response.Content != null)
                {
                    rs.Content = await rs.Response.Content.ReadAsStringAsync();
                }

                rs.IsSuccess = rs.Response.IsSuccessStatusCode;

                if(mapper != null)
                    mapper(rs);
            }
            catch(TaskCanceledException tcex)
            {
                rs.Ex = tcex;
                rs.IsTimeout = true;
                rs.IsSuccess = false;
            }
            catch(Exception ex)
            {
                rs.Ex = ex;
                rs.IsSuccess = false;
            }

            return rs;
        }

        private ApimPrincipal CreatePrincipal(ApimUserDetail userDetail)
        {
            var identity = new ApimIdentity
            {
                IsAuthenticated = true,
                Id = userDetail.id,
                Name = string.Format("{0} {1}", userDetail.firstName, userDetail.lastName),
                Email = userDetail.email
            };

            string[] roles = new string[0];

            if (string.IsNullOrEmpty(userDetail.note))
                return new ApimPrincipal(identity, roles);

            var userProperties = userDetail.note.Split(';')
                .Where(up => !string.IsNullOrEmpty(up) && up.Contains("="))
                .ToDictionary(v => v.Split('=')[0], v => v.Split('=')[1], StringComparer.InvariantCultureIgnoreCase);

            string source;
            if (userProperties.TryGetValue("Source", out source))
                identity.Source = source;

            string role;
            if (userProperties.TryGetValue("Roles", out role))
            {
                roles = role.Split('|');
            }

            return new ApimPrincipal(identity, roles);
        }
    }
}
