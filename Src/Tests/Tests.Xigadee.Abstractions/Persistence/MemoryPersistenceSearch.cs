﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Tests.Xigadee
{
    [TestClass]
    public class TestMemoryPersistenceSearch
    {
        #region TestClass 
        private class TestClass
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public Guid VersionId { get; set; } = Guid.NewGuid();

            /// <summary>
            /// Gets or sets the unique name.
            /// </summary>
            public string Name { get; set; }

            public decimal Amount { get; set; }

            public int Group => decimal.ToInt32(Amount) % 10;

            public DateTime DateCreated { get; set; } = DateTime.UtcNow;

            public DateTime? DateUpdated { get; set; }
        }

        private IEnumerable<Tuple<string, string>> References(TestClass c)
        {
            yield return new Tuple<string, string>("name", c.Name);
        }

        private IEnumerable<Tuple<string, string>> Properties(TestClass c)
        {
            yield return new Tuple<string, string>("type", c.Amount.ToString());
            yield return new Tuple<string, string>("datecreated", c.DateCreated.ToString("o"));
            yield return new Tuple<string, string>("group", c.Group.ToString());
        }

        #endregion


        IRepositoryAsyncServer<Guid, TestClass> GetRepo()
        {
            var repo = new RepositoryMemory<Guid, TestClass>(r => r.Id
            , (e) => References(e)
            , (e) => Properties(e)
            , versionPolicy: ((TestClass e) => e.VersionId.ToString("N"), (TestClass e) => e.VersionId = Guid.NewGuid())
                );

            repo.SearchAdd(new RepositoryMemorySearch<Guid, TestClass>("default"));

            for (int i = 0; i < 100; i++)
            {
                var rs = repo.Create(new TestClass() { Amount = i, Name = $"Id{i}" }).Result;
            }

            return repo;
        }

        [TestMethod]
        public async Task SearchEntity()
        {
            var repo = GetRepo();

            
            var item1 = await repo.ReadByRef("Name", "Id55");
            var e = item1.Entity;
            e.Name = "Paul123";
            var resChange = await repo.Update(e);

            var sr = new SearchRequest() { Id = "default" };
            var sr2 = (SearchRequest)"$top=100&$id=default&$skip=3&$select=Type,Group,DateCreated&$orderby=Group desc, Type desc";
            var str = sr2.ToString();
            var res1 = await repo.SearchEntity(sr2);
            var res2 = await repo.Search(sr2);

        }
    }
}
