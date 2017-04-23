using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class ServiceIdentifierValidation
    {
        [TestMethod]
        public void IsValid()
        {
            Assert.IsTrue(MicroserviceId.ValidServiceIdentifier("Freddy"));
            Assert.IsTrue(MicroserviceId.ValidServiceIdentifier("freddy12345"));
            Assert.IsTrue(MicroserviceId.ValidServiceIdentifier("23freddy12345"));
        }

        [TestMethod]
        public void IsNotValid()
        {
            Assert.IsFalse(MicroserviceId.ValidServiceIdentifier("Freddy Got Fingered"));
            Assert.IsFalse(MicroserviceId.ValidServiceIdentifier("Freddy*"));
            Assert.IsFalse(MicroserviceId.ValidServiceIdentifier(" Freddy"));
        }

        [TestMethod]
        public void IsCorrect()
        {
            try
            {
                var ids = new MicroserviceId("Correct","Amondo");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [TestMethod]
        public void IsError1()
        {
            try
            {
                var ids = new MicroserviceId("Correct*", "Amondo");
                Assert.Fail();
            }
            catch (MicroserviceIdNotValidException midex)
            {
                //This is OK.
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [TestMethod]
        public void IsError2()
        {
            try
            {
                var ids = new MicroserviceId("Correct", "Amondo_");
                Assert.Fail();
            }
            catch (MicroserviceIdNotValidException midex)
            {
                //This is OK.
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
