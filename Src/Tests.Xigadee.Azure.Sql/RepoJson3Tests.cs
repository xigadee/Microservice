using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee.Azure.Sql
{
    [TestClass]
    public class RepoJson3Test
    {
        [TestMethod]
        public void WrapperTest()
        {
            var t = new Test1();
            
            t.UserId = Guid.NewGuid();
            t.AccountId = Guid.NewGuid();
            var ms = DateTime.UtcNow.Millisecond;

            if (ms > 100)
                t.Second = DateTime.UtcNow.Millisecond;

            var wr = new SqlJsonWrapper<Test1>(t);

            var json = JsonConvert.SerializeObject(wr);

            var wrdeser = JsonConvert.DeserializeObject<SqlJsonWrapper<Test1>>(json);
        }

    }
}
