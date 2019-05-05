using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xigadee;

namespace Tests.Xigadee.Azure.Sql
{
    [TestClass]
    public class SqlGen
    {
        [TestMethod]
        public void Verify_SqlStoredProcedureResolver()
        {

            var spNames2 = new SqlStoredProcedureResolver<User>(schemaName: "External", interfix: "_");
            var generator2 = new RepositorySqlJsonGenerator<User>(spNames2);

            //Assert.AreEqual(spNames.StoredProcedureCreate, "[External].spCreate_Test1");
            //Assert.AreEqual(spNames.StoredProcedureSearch("default"), "mysearch1default");

            var spNames = new SqlStoredProcedureResolver<Test1>(schemaName: "External", interfix: "_");
            var generator = new RepositorySqlJsonGenerator<Test1>(spNames);

            var sql1 = generator.TableEntity;
            var sql2 = generator.TableHistory;
            var sql3 = generator.TableProperty;
            var sql4 = generator.TablePropertyKey;
            var sql5 = generator.TableReference;
            var sql6 = generator.TableReferenceKey;

            var tab = generator.ScriptTable();

            var sql7 = generator.AncillarySchema;
            var sql8 = generator.AncillaryKvpTableType;
            var sql9 = generator.AncillaryIdTableType;

            var sp1 = generator.SpCreate;
            var sp2 = generator.SpDelete;
            var sp3 = generator.SpDeleteByRef;
            var sp4 = generator.SpUpdate;
            var sp5 = generator.SpRead;
            var sp6 = generator.SpReadByRef;
            var sp7 = generator.SpUpsertRP;
            var sp8 = generator.SpVersion;
            var sp9 = generator.SpVersionByRef;

            var spsearch = generator.SpSearch;
            var spsearchb = generator.SpSearchEntity;
            var spsearche = generator.SpSearchBuild;

            var sps = generator.ScriptStoredProcedures(true);
            var spc = generator.ScriptStoredProcedures(false);

            var anc = generator.ScriptAncillary();

            var scr = generator.ScriptEntity();

            var scr2a = generator2.ScriptAncillary();
            var scr2e = generator2.ScriptEntity();

        }
    }
}
