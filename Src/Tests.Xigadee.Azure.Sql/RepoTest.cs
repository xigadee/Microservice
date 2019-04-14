using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee.Azure.Sql
{
    [TestClass]
    public class RepoTest
    {

        [TestMethod]
        public async Task SqlJsonTest()
        {
            var server = "tpjrtest";
            var uname = "ApiService";
            var pwd = "123Enter**&";
            var catalog = "xgtest2";
            var conn = $"Server=tcp:{server}.database.windows.net,1433;Initial Catalog={catalog};Persist Security Info=False;User ID={uname};Password={pwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            var spNames2 = new SqlStoredProcedureResolver<User>(schemaName: "External", interfix: "_");

            var repo = new RepositorySqlJson<Guid, User>(conn, spNamer: spNames2);

            var newUser = new User();
            newUser.PropertiesSet("email", "pstancer@outlook.com");

            try
            {
                var attempt1 = await repo.Create(newUser);
                var read1 = await repo.Read(newUser.Id);
                newUser.PropertiesSet("wahey", "123");
                var attempt2 = await repo.Update(newUser);
                var attempt3 = await repo.Update(newUser);
                var v1 = await repo.Version(newUser.Id);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
