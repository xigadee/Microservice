using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Xigadee;
using Xigadee;

namespace Tests.Xigadee
{

    [TestClass]
    public class TestMemoryPersistenceCheck
    {
        IRepositoryAsync<Guid, TestClass> _repo;

        private IEnumerable<Tuple<string, string>> References(TestClass c)
        {
            yield return new Tuple<string, string>("name", c.Name);
        }

        private IEnumerable<Tuple<string, string>> Properties(TestClass c)
        {
            yield return new Tuple<string, string>("type", c.Type);
            yield return new Tuple<string, string>("datecreated", c.DateCreated.ToString("o"));
        }


        [TestInitialize]
        public void Init()
        {
            var prov = AesWrapperSignaturePolicyWrapper.CreateTestPolicy();

            _repo = new RepositoryMemory<Guid, TestClass>((r) => r.Id
                , referenceMaker: References
                , propertiesMaker: Properties
                , versionPolicy: new VersionPolicy<TestClass>(
                    (e) => e.VersionId.ToString("N").ToUpperInvariant()
                    , (e) => e.VersionId = Guid.NewGuid()
                )
                , signaturePolicy: prov
                );
        }

        [TestMethod]
        public async Task Create()
        {
            var result2 = await _repo.Create(new TestClass() { Name = "Ferd", Type = "nerd" });
            var result3 = await _repo.Create(new TestClass() { Name = "Freda", Type = "nerd" });

            //This will fail as it has the same 'Name' as an earlier entity.
            var result4 = await _repo.Create(new TestClass() { Name = "Ferd", Type = "geek" });

            var result5 = await _repo.Create(new TestClass() { Name = "Ferdy", Type = "geek" });

            Assert.IsTrue(result2.IsSuccess);
            Assert.IsTrue(result3.IsSuccess);
            Assert.IsFalse(result4.IsSuccess);
            Assert.IsTrue(result5.IsSuccess);

            //var result = await _repo.Search();
            //Assert.IsTrue(result.Entity.Data.Count == 3);

            //OK, we just need to confirm the update.
            var resRead2 = await _repo.Read(result2.Entity.Id);
            var entity = resRead2.Entity;
            entity.Name = Guid.NewGuid().ToString("N");

            var rsUpdate = await _repo.Update(entity);
            Assert.IsTrue(rsUpdate.IsSuccess);

            var resRead3 = await _repo.Read(entity.Id);
            Assert.IsTrue(resRead3.IsSuccess);

        }


        [TestMethod]
        public async Task Update()
        {
            var e1 = new TestClass() { Name = "Gangly" };

            var result = await _repo.Create(e1);

            Assert.IsTrue(result.IsSuccess);

            var e2 = result.Entity;

            e2.DateUpdated = DateTime.UtcNow;

            var result2 = await _repo.Update(e2);

            Assert.IsTrue(result2.IsSuccess);

            //This should now fail as the version id has been changed by the earlier update.
            var result3 = await _repo.Update(e2);

            Assert.IsTrue(!result3.IsSuccess);
        }


        [TestMethod]
        public async Task Delete()
        {
            var result = await _repo.Create(new TestClass() { Name = "Ismail" });

            Assert.IsTrue(result.IsSuccess);

            var entity = result.Entity;

            var resultr1 = await _repo.Read(entity.Id);
            Assert.IsTrue(resultr1.IsSuccess);

            var result2 = await _repo.Delete(entity.Id);
            Assert.IsTrue(result2.IsSuccess);

            var resultr2 = await _repo.Read(entity.Id);
            Assert.IsFalse(resultr2.IsSuccess);

        }
    }
}
