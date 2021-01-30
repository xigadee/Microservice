using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Xigadee.AspNetCore50
{
    [TestClass]
    public class AspNetTest1 : TestServerUnitTestBase
    {
        [TestMethod]
        public void Init1()
        {
            var prov = ConnectorGetLocal();

        }
    }
}
