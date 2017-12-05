using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Serialization
{
    [TestClass]
    public class SerializationContainerTests
    {
        #region SerializationTest1()
        /// <summary>
        /// Exception is thrown if no serializers have been set.
        /// </summary>
        [TestMethod]
        public void SerializationTest1()
        {
            var harness = new SerializationContainerHarness();

            var sr = harness.Service;

            var srJson = sr.Add(new JsonContractSerializer());

            try
            {
                harness.Start();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }           
        }
        #endregion

        #region SerializationTestAddClear()
        /// <summary>
        /// Check add/remove functionality for the container.
        /// </summary>
        [TestMethod]
        public void SerializationTestAddClear()
        {
            var harness = new SerializationContainerHarness();

            var sr = harness.Service;

            sr.Add(new JsonContractSerializer());

            Assert.AreEqual(sr.Count, 1);

            sr.Clear();
            Assert.AreEqual(sr.Count, 0);

            sr.Add(new JsonContractSerializer());
            Assert.AreEqual(sr.Count, 1);

            try
            {
                harness.Start();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        #endregion

        #region SerializationTestEmptyException()
        /// <summary>
        /// Exception is thrown if no serializers have been set.
        /// </summary>
        [TestMethod]
        public void SerializationTestEmptyException()
        {
            var harness = new SerializationContainerHarness();

            try
            {
                harness.Start();
            }
            catch (PayloadSerializerCollectionIsEmptyException pex)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        } 
        #endregion
    }
}
