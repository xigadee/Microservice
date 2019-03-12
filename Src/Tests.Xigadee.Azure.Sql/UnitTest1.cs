using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xigadee;

namespace Tests.Xigadee.Azure.Sql
{
    [TestClass]
    public class SqlStuff
    {
        private class Test1
        {
            public Guid Id { get; set; } = Guid.NewGuid();
        }

        [TestMethod]
        public void Verify_SqlStoredProcedureResolver()
        {
            var spNames = new SqlStoredProcedureResolver<Test1>(schemaName:"External", interfix:"_"
                , overrides: new[] { (RepositoryMethod.Search, "mysearch1") });

            Assert.AreEqual(spNames.StoredProcedureCreate, "[External].spCreate_Test1");
            Assert.AreEqual(spNames.StoredProcedureSearch, "mysearch1");
        }
    }
}
