using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Xigadee;

namespace Test.Xigadee.Api
{
    [TestClass]
    public class JwtTests:JwtTestBase
    {
        #region Constants
        public const string SecretPass = "Jwt is cool";
        public const string SecretFail = "Jwt is whack";

        public const string Audience = "JWTTests";
        public const string Username = "Paul";
        public const string Role = "CoolCoder";
        #endregion
        #region Declarations
        private byte[] mSecret = Encoding.ASCII.GetBytes(SecretPass);
        private byte[] mSecretFail = Encoding.ASCII.GetBytes(SecretFail);
        #endregion

        #region Initialization
        [TestInitialize]
        public override void FixtureInit()
        {
            base.FixtureInit();
        }

        [TestCleanup]
        public override void FixtureCleanUp()
        {
            base.FixtureCleanUp();
        }
        #endregion

        #region GetToken()
        /// <summary>
        /// This method gets the default token.
        /// </summary>
        /// <returns>Returns a JwtToken with a set of default claims.</returns>
        private JwtToken GetToken()
        {
            var token = new JwtToken();

            token.Claims.Audience = Audience;

            token.Claims.NotBefore = DateTime.UtcNow.AddHours(-1);
            token.Claims.ExpirationTime = DateTime.UtcNow.AddHours(1);
            token.Claims.IssuedAt = DateTime.UtcNow;
            token.Claims.JWTId = Guid.NewGuid().ToString("N");

            token.Claims.ShortcutSetRole(Role);
            token.Claims.ShortcutSetName(Username);

            return token;
        } 
        #endregion

        [TestMethod]
        public void TestMethodSuccess1()
        {
            var token = GetToken();

            var response = ReadWithRetry(token.ToString(mSecret),() => new HttpRequestMessage(HttpMethod.Get, "/api/test"));

            var result = response.Content.ReadAsAsync<IEnumerable<string>>().Result;

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("hello", result.First());
            Assert.AreEqual("world", result.Last());
        }

        [TestMethod]
        public void TestMethodSuccess_ValidateRoleAndUsername1()
        {
            var token = GetToken();

            int id = 2;

            var response = ReadWithRetry(token.ToString(mSecret), () => new HttpRequestMessage(HttpMethod.Get, $"/api/test/{id}"));

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);

            var result = response.Content.ReadAsAsync<string>().Result;

            Assert.AreEqual(result, $"super hello world {id} {Username}");
        }

        [TestMethod]
        public void TestMethodSuccess_ValidateRoleAndUsername2()
        {
            var token = GetToken();
            token.Claims.ShortcutSetRole("BOring");

            int id = 2;

            var response = ReadWithRetry(token.ToString(mSecret), () => new HttpRequestMessage(HttpMethod.Get, $"/api/test/{id}"));

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);

            var result = response.Content.ReadAsAsync<string>().Result;

            Assert.AreEqual(result, $"hello world {id} {Username}");
        }

        [TestMethod]
        public void TestMethodFail403_NoToken_DenyByDefault()
        {
            var response = ReadWithRetry(null, () => new HttpRequestMessage(HttpMethod.Get, "/api/test"));

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void TestMethodFail403_BadSecret()
        {
            var token = GetToken();

            var response = ReadWithRetry(token.ToString(mSecretFail), () => new HttpRequestMessage(HttpMethod.Get, "/api/test"));

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void TestMethodFail403_TokenExpired()
        {
            var token = GetToken();
            token.Claims.ExpirationTime = DateTime.UtcNow.AddHours(-1);

            var response = ReadWithRetry(token.ToString(mSecretFail), () => new HttpRequestMessage(HttpMethod.Get, "/api/test"));

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void TestMethodFail403_TokenNotYetValid()
        {
            var token = GetToken();
            token.Claims.NotBefore = DateTime.UtcNow.AddMinutes(30);

            var response = ReadWithRetry(token.ToString(mSecretFail), () => new HttpRequestMessage(HttpMethod.Get, "/api/test"));

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void TestMethodFail403_BadAudience()
        {
            var token = GetToken();
            token.Claims.Audience = "notgood";

            var response = ReadWithRetry(token.ToString(mSecretFail), () => new HttpRequestMessage(HttpMethod.Get, "/api/test"));

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }
    }

    /// <summary>
    /// This test controller is used to verify connectivity.
    /// </summary>
    public class TestController: ApiController
    {
        public IEnumerable<string> Get()
        {
            var principal = Thread.CurrentPrincipal;
            return new[] { "hello", "world" };
        }

        [WebApiCircuitBreakerFilter("Jeremy")]
        public string Get(int id)
        {
            if (Thread.CurrentPrincipal.IsInRole(JwtTests.Role))
                return $"super hello world {id} {Thread.CurrentPrincipal?.Identity?.Name}";
            else
                return $"hello world {id} {Thread.CurrentPrincipal?.Identity?.Name}";
        }
    }

}
