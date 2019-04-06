using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xigadee;

namespace Tests.Xigadee.Azure.Sql
{
    [TestClass]
    public class SqlTests
    {
        private class Test1:EntityAuditableBase
        {

        }

        [TestMethod]
        public void Verify_SqlStoredProcedureResolver()
        {
            var spNames = new SqlStoredProcedureResolver<Test1>(schemaName:"External", interfix:"_"
                , overrides: new[] { (RepositoryMethod.Search, "mysearch1") });

            Assert.AreEqual(spNames.StoredProcedureCreate, "[External].spCreate_Test1");
            Assert.AreEqual(spNames.StoredProcedureSearch("default"), "mysearch1default");

            var generator = new RepositorySqlJsonGenerator<Test1>(spNames);

            var tableSql = generator.TableEntity;
        }
    }
}
