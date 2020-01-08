using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee.Azure.Sql
{
    [TestClass]
    public class RepoTest
    {
        #region TestContext
        /// <summary>
        /// All hail the Microsoft test magic man!
        /// With VS2017, this class can be populated with values through Visual Studio menus -> [Test>Test Settings>Select Test Settings File] and then selecting a file with the extension .runsettings
        /// See here for details: https://msdn.microsoft.com/en-us/library/jj635153.aspx
        /// There is a default file default.runsettings that has a set of empty CI injection parameters specified for testing in this project.
        /// </summary>
        public TestContext TestContext
        {
            get; set;
        }
        #endregion

        #region BuildSqlConnection()
        private string BuildSqlConnection()
        {
            var server = TestContext.Properties["sqlServer"].ToString();
            var uname = TestContext.Properties["sqlUser"].ToString();
            var pwd = TestContext.Properties["sqlPwd"].ToString();
            var catalog = TestContext.Properties["sqlCatalog"].ToString();

            var conn = $"Server=tcp:{server}.database.windows.net,1433;Initial Catalog={catalog};Persist Security Info=False;User ID={uname};Password={pwd};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            return conn;
        }
        #endregion

        #region BuildAesSignatureWrapper()
        private AesSha512SignaturePolicyWrapper BuildAesSignatureWrapper()
        {
            var keyStr = TestContext.Properties["aesKey"].ToString();
            var ivStr = TestContext.Properties["aesIv"].ToString();

            var key = Convert.FromBase64String(keyStr);
            var iv = Convert.FromBase64String(ivStr);

            return new AesSha512SignaturePolicyWrapper(key,iv);
        }
        #endregion

        readonly Guid[] Accounts = { new Guid("{C9BD832A-DFDC-41D6-934C-853EC1272C37}"), new Guid("{48693BD8-05D2-49D0-BCAA-4B588303D2C6}") };

        [TestMethod]
        public async Task CreateKey()
        {
            var provider = AesSha512SignaturePolicyWrapper.CreateTestPolicy();

            var key = Convert.ToBase64String(provider.Provider.Key);
            var iv = Convert.ToBase64String(provider.Provider.IV);
        }

        [TestMethod]
        public async Task SqlJsonTest1Test()
        {
            var conn = BuildSqlConnection();

            var repo = new RepositorySqlJson2<Guid, Test1>(conn, signaturePolicy: new SignaturePolicyNull());

            for (int i = 0; i < 100; i++)
            {
                RepositoryHolder<Guid, Test1> rs;
                try
                {
                    var t = new Test1();

                    t.UserId = Guid.NewGuid();
                    t.AccountId = Accounts[i % Accounts.Length];
                    var ms = DateTime.UtcNow.Millisecond;

                    if (ms > 100)
                        t.Second = DateTime.UtcNow.Millisecond;

                    rs = await repo.Create(t);

                    var rsCheck = await repo.Read(t.Id);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [TestMethod]
        public async Task SqlJsonTest1TestSignature()
        {
            var conn = BuildSqlConnection();

            var repo = new RepositorySqlJson2<Guid, Test1>(conn, signaturePolicy: BuildAesSignatureWrapper());

            for (int i = 0; i < 100; i++)
            {
                RepositoryHolder<Guid, Test1> rs;
                try
                {
                    var t = new Test1();

                    t.UserId = Guid.NewGuid();
                    t.AccountId = Accounts[i % Accounts.Length];
                    var ms = DateTime.UtcNow.Millisecond;

                    if (ms > 100)
                        t.Second = DateTime.UtcNow.Millisecond;

                    rs = await repo.Create(t);

                    var rsCheck = await repo.Read(t.Id);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [TestMethod]
        public async Task SqlJsonTest1Search()
        {
            var conn = BuildSqlConnection();

            var repo = new RepositorySqlJson2<Guid, Test1>(conn);

            try
            {
                var srq1 = (SearchRequest)$"$top=50&$id=default&$orderby=DateCombined desc&$filter=accountid eq '{Accounts[0]}' and second eq 244";

                var s1 = await repo.SearchEntity(srq1);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public async Task SqlJsonTest1Search2()
        {
            var conn = BuildSqlConnection();

            var repo = new RepositorySqlJson2<Guid, Test1>(conn);

            try
            {
                var srq1 = (SearchRequest)$"$top=50&$id=default&$skip=5&$orderby=DateCombined desc";

                var s1 = await repo.SearchEntity(srq1);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public async Task SqlJsonTest()
        {
            var conn = BuildSqlConnection();

            var repo = new RepositorySqlJson2<Guid, User>(conn);
            var repom = new RepositoryMemory<Guid, User>((u) => u.Id);

            var newUser = new User();
            newUser.PropertiesSet("email", "pstancer@outlook.com");

            try
            {
                //var attempt1 = await repo.Create(newUser);
                //var attempt1m = await repom.Create(newUser);
                //var read1 = await repo.Read(newUser.Id);
                //var read1m = await repom.Read(newUser.Id);
                //newUser.PropertiesSet("wahey", "123");
                //var attempt2 = await repo.Update(newUser);
                //var attempt2m = await repom.Update(newUser);
                //var attempt3 = await repo.Update(newUser);
                //var v1 = await repo.Version(newUser.Id);
                //var v1m = await repom.Version(newUser.Id);

                var srq1 = (SearchRequest)"$top=100&$id=default&$skip=3&$select=Type,Group,DateCreated&$orderby=Group desc, Type desc&$filter=id eq '123' and ertp gt 610 and CustomerID eq null";

                var s1 = await repo.SearchEntity(srq1);
                var s2 = await repo.Search(srq1);
                var s1m = await repom.SearchEntity(new SearchRequest());
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
