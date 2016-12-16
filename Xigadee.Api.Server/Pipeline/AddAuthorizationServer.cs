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
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Xigadee
{
    public static partial class WebApiExtensionMethods
    {
        public static P AddJWTAuthorization<P>(this P webpipe, bool removeUnderlyingPrincipal = true)
            where P : IPipelineWebApi
        {
            //Remove any auth created by the underlying fabric.
            if (removeUnderlyingPrincipal)
                webpipe.HttpConfig.SuppressHostPrincipal();


            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("sub", "stano"));
            identity.AddClaim(new Claim("role", "user"));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = principal;

            //context.Validated(identity);

            return webpipe;
        }
    }

    //public class ITokenAuth: IAuthenticationFilter
    //{
    //    public bool AllowMultiple
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
    //    {
    //        // 1. Look for credentials in the request.
    //        HttpRequestMessage request = context.Request;
    //        AuthenticationHeaderValue authorization = request.Headers.Authorization;

    //        // 2. If there are no credentials, do nothing.
    //        if (authorization == null)
    //        {
    //            return;
    //        }

    //        // 3. If there are credentials but the filter does not recognize the 
    //        //    authentication scheme, do nothing.
    //        if (authorization.Scheme != "Basic")
    //        {
    //            return;
    //        }

    //        // 4. If there are credentials that the filter understands, try to validate them.
    //        // 5. If the credentials are bad, set the error result.
    //        if (String.IsNullOrEmpty(authorization.Parameter))
    //        {
    //            context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
    //            return;
    //        }

    //        Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);
    //        if (userNameAndPasword == null)
    //        {
    //            context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
    //        }

    //        string userName = userNameAndPasword.Item1;
    //        string password = userNameAndPasword.Item2;

    //        IPrincipal principal = await AuthenticateAsync(userName, password, cancellationToken);
    //        if (principal == null)
    //        {
    //            context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
    //        }

    //        // 6. If the credentials are valid, set principal.
    //        else
    //        {
    //            context.Principal = principal;
    //        }

    //    }

    //    public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class AuthenticationFailureResult: IHttpActionResult
    {
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }

    public class PinBasedAuthenticationOptions: AuthenticationOptions
    {
        public PinBasedAuthenticationOptions() : base("x-company-auth")
        { }
    }

    public class PinAuthenticationHandler: AuthenticationHandler<PinBasedAuthenticationOptions>
    {
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            bool authorized = await Task<bool>.Run(() => IsAuthorised(Request.Headers));
            if (authorized)
            {
                AuthenticationProperties authProperties = new AuthenticationProperties();
                authProperties.IssuedUtc = DateTime.UtcNow;
                authProperties.ExpiresUtc = DateTime.UtcNow.AddDays(1);
                authProperties.AllowRefresh = true;
                authProperties.IsPersistent = true;
                IList<Claim> claimCollection = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Andras")
                    , new Claim(ClaimTypes.Country, "Sweden")
                    , new Claim(ClaimTypes.Gender, "M")
                    , new Claim(ClaimTypes.Surname, "Nemes")
                    , new Claim(ClaimTypes.Email, "hello@me.com")
                    , new Claim(ClaimTypes.Role, "IT")
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claimCollection, "Custom");
                AuthenticationTicket ticket = new AuthenticationTicket(claimsIdentity, authProperties);
                return ticket;
            }

            return null;
        }

        private bool IsAuthorised(IHeaderDictionary requestHeaders)
        {
            string[] acceptLanguageValues;
            bool acceptLanguageHeaderPresent = requestHeaders.TryGetValue("x-company-auth", out acceptLanguageValues);
            if (acceptLanguageHeaderPresent)
            {
                string[] elementsInHeader = acceptLanguageValues.ToList()[0].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (elementsInHeader.Length == 2)
                {
                    int pin;
                    if (int.TryParse(elementsInHeader[1], out pin))
                    {
                        if (pin >= 500000)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class SomethingHandler: AuthenticationHandler
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            throw new NotImplementedException();
        }
    }
}
