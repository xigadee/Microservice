using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xigadee;

namespace Test.Xigadee.Security
{
    [TestClass]
    public class JWTUnitTests
    {
        static byte[] exampleHeader = new byte[] {
            123, 34, 116, 121, 112, 34, 58, 34, 74, 87, 84, 34, 44, 13, 10, 32,
            34, 97, 108, 103, 34, 58, 34, 72, 83, 50, 53, 54, 34, 125 };

        static byte[] exampleClaims = new byte[] {
            123, 34, 105, 115, 115, 34, 58, 34, 106, 111, 101, 34, 44, 13, 10,
            32, 34, 101, 120, 112, 34, 58, 49, 51, 48, 48, 56, 49, 57, 51, 56,
            48, 44, 13, 10, 32, 34, 104, 116, 116, 112, 58, 47, 47, 101, 120, 97,
            109, 112, 108, 101, 46, 99, 111, 109, 47, 105, 115, 95, 114, 111,
            111, 116, 34, 58, 116, 114, 117, 101, 125};

        static string check = "eyJ0eXAiOiJKV1QiLA0KICJhbGciOiJIUzI1NiJ9";
        static string check2 = "eyJpc3MiOiJqb2UiLA0KICJleHAiOjEzMDA4MTkzODAsDQogImh0dHA6Ly9leGFtcGxlLmNvbS9pc19yb290Ijp0cnVlfQ";


        [TestMethod]
        public void ClaimsSetTest()
        {
            try
            {
                var set = new ClaimsSet("{\"typ\":\"JWT\",      \"alg\":\"HS256\"}");

                var keys = set.ToList();

                set["awkward"] = (int)24;
                set["typ"] = "Scooby";
                set["hmm"] = 42;

                var json = set.ToString();

                var set2 = new ClaimsSet(json);

                Assert.AreEqual(set2["typ"], "Scooby");
                Assert.AreEqual(set2["alg"], "HS256");
                Assert.AreEqual((long)set2["awkward"], 24L);
                Assert.AreEqual((long)set2["hmm"], 42L);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [TestMethod]
        public void ValidateParsing()
        {           
            var id = "eyJhbGciOiJSU0ExXzUiLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwiY3R5IjoiSldUIn0.g_hEwksO1Ax8Qn7HoN-BVeBoa8FXe0kpyk_XdcSmxvcM5_P296JXXtoHISr_DD_MqewaQSH4dZOQHoUgKLeFly-9RI11TG-_Ge1bZFazBPwKC5lJ6OLANLMd0QSL4fYEb9ERe-epKYE3xb2jfY1AltHqBO-PM6j23Guj2yDKnFv6WO72tteVzm_2n17SBFvhDuR9a2nHTE67pe0XGBUS_TK7ecA-iVq5COeVdJR4U4VZGGlxRGPLRHvolVLEHx6DYyLpw30Ay9R6d68YCLi9FYTq3hIXPK_-dmPlOUlKvPr1GgJzRoeC9G5qCvdcHWsqJGTO_z3Wfo5zsqwkxruxwA.UmVkbW9uZCBXQSA5ODA1Mg.VwHERHPvCNcHHpTjkoigx3_ExK0Qc71RMEParpatm0X_qpg-w8kozSjfNIPPXiTBBLXR65CIPkFqz4l1Ae9w_uowKiwyi9acgVztAi-pSL8GQSXnaamh9kX1mdh3M_TT-FZGQFQsFhu0Z72gJKGdfGE-OE7hS1zuBD5oEUfk0Dmb0VzWEzpxxiSSBbBAzP10l56pPfAtrjEYw-7ygeMkwBl6Z_mLS6w6xUgKlvW6ULmkV-uLC4FUiyKECK4e3WZYKw1bpgIqGYsw2v_grHjszJZ-_I5uM-9RA8ycX9KqPRp9gc6pXmoU_-27ATs9XCvrZXUtK2902AUzqpeEUJYjWWxSNsS-r1TJ1I-FMJ4XyAiGrfmo9hQPcNBYxPz3GQb28Y5CLSQfNgKSGt0A4isp1hBUXBHAndgtcslt7ZoQJaKe_nNJgNliWtWpJ_ebuOpEl8jdhehdccnRMIwAmU1n7SPkmhIl1HlSOpvcvDfhUN5wuqU955vOBvfkBOh5A11UzBuo2WlgZ6hYi9-e3w29bR0C2-pp3jbqxEDw3iWaf2dc5b-LnR0FEYXvI_tYk5rd_J9N0mg0tQ6RbpxNEMNoA9QWk5lgdPvbh9BaO195abQ.AVO9iT5AV4CzvDJCdhSFlQ";

            var jwtbase = new JwtRoot(id);

            Assert.AreEqual(id, jwtbase.ToJWSCompactSerialization());
        }

        [TestMethod]
        public void JwtTokenTest1()
        {
            var check1 = JwtHelper.SafeBase64UrlEncode(Encoding.UTF8.GetBytes("{\"typ\":\"JWT\",      \"alg\":\"HS256\"}"));

            var jwtbase = new JwtToken($"{check1}.{check2}", null);
            var algo = jwtbase.Header.SupportedAlgorithm;
        }

        [TestMethod]
        public void ValidateNames()
        {
            byte[] key = Guid.NewGuid().ToByteArray();
            string keyCheck = Convert.ToBase64String(key);

            Assert.IsTrue(JwtRoot.GetAlgorithm(JwtHelper.ConvertToJwtHashAlgorithm("HS256"), key) is HMACSHA256);
            Assert.IsTrue(JwtRoot.GetAlgorithm(JwtHelper.ConvertToJwtHashAlgorithm("HS384"), key) is HMACSHA384);
            Assert.IsTrue(JwtRoot.GetAlgorithm(JwtHelper.ConvertToJwtHashAlgorithm("HS512"), key) is HMACSHA512);

        }


        [TestMethod]
        public void JwtTokenTest2()
        {
            var token = new JwtToken();
            var secret = Guid.NewGuid().ToByteArray();

            token.Claims[JwtClaims.HeaderIssuer] = "stano";

            var signed = token.ToString(secret);

            var token2 = new JwtToken(signed, secret);

            var token3 = new JwtToken(signed, null, false);

            Assert.IsTrue(token3.ValidateIncoming(secret));
            Assert.IsFalse(token3.ValidateIncoming(Guid.NewGuid().ToByteArray()));
        }

        [TestMethod]
        public void JwtTokenTest3()
        {
            var key = Encoding.ASCII.GetBytes("616161A");
            var id = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2p3dC1pZHAuZXhhbXBsZS5jb20iLCJzdWIiOiJtYWlsdG86bWlrZUBleGFtcGxlLmNvbSIsIm5iZiI6MTQ4Mzg3MjcyMSwiZXhwIjoxNDgzODc2MzIxLCJpYXQiOjE0ODM4NzI3MjEsImp0aSI6ImlkMTIzNDU2IiwidHlwIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9yZWdpc3RlciJ9.bMdGRvtLXSzZvF_3vlJ1T8DQ_Uc6AOa0Fr9-p8pU3UI";
            var token = new JwtToken(id, null, false);

            var result = token.ValidateIncoming(key);
        }


        [TestMethod]
        public void JwtTokenTest4()
        {
            var key = Encoding.UTF8.GetBytes("616161A");
            var id = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2p3dC1pZHAuZXhhbXBsZS5jb20iLCJzdWIiOiJtYWlsdG86bWlrZUBleGFtcGxlLmNvbSIsIm5iZiI6MTQ4Mzg3MjcyMSwiZXhwIjoxNDgzODc2MzIxLCJpYXQiOjE0ODM4NzI3MjEsImp0aSI6ImlkMTIzNDU2IiwidHlwIjoiaHR0cHM6Ly9leGFtcGxlLmNvbS9yZWdpc3RlciJ9.bMdGRvtLXSzZvF_3vlJ1T8DQ_Uc6AOa0Fr9-p8pU3UI";
            var token = new JwtToken(id, null, false);

            var result = token.ValidateIncoming(key);

            token.Claims.IssuedAt = DateTime.UtcNow;
        }


        [TestMethod]
        public void JwtTokenTest5()
        {
            var key = Encoding.ASCII.GetBytes("SuperSecret");
            var id = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpYXQiOjE0NjQzNDg4MTcsInVzZXIiOiJqZXJvZW4iLCJzZXNzaW9uX2tleSI6MTIzNDU2fQ.HvR8WTLm7d5lfuPCH7vC9RjKliWOoljXScIAoshm1YM";
            var token = new JwtToken(id, key, true);
        }

    }
}
