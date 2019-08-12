using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        /// This class can be populated with values through Visual Studio menus -> [Test>Test Settings>Select Test Settings File] and then selecting a file with the extension .runsettings
        /// See here for details: https://msdn.microsoft.com/en-us/library/jj635153.aspx
        /// There is a default file default.runsettings that has a set of empty CI injection parameters specified for testing in this project.
        /// </summary>
        public TestContext TestContext
        {
            get; set;
        }
        #endregion

        public string BuildSqlConnection()
        {
            var server = TestContext.Properties["sqlServer"].ToString();
            var uname = TestContext.Properties["sqlUser"].ToString();
            var pwd = TestContext.Properties["sqlPwd"].ToString();
            var catalog = TestContext.Properties["sqlCatalog"].ToString();

            var conn = $"Server=tcp:{server}.database.windows.net,1433;Initial Catalog={catalog};Persist Security Info=False;User ID={uname};Password={pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            return conn;
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
                var s2 = await repo.Search(new SearchRequest());
                var s1m = await repom.SearchEntity(new SearchRequest());
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
