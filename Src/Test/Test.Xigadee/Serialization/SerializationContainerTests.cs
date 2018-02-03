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
            var harness = new ServiceHandlerContainerHarness();

            var sr = harness.Service;

            var srJson = sr.Serialization.Add(new JsonContractSerializer());
            //var srBson = sr.Add(new BsonContractSerializer());
            //var srJsonRaw = sr.Add(new JsonRawSerializer());
            //var srBsonRaw = sr.Add(new BsonRawSerializer());

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
            var harness = new ServiceHandlerContainer();

            harness.Serialization.Add(new JsonContractSerializer());

            Assert.AreEqual(harness.Serialization.Count, 1);

            harness.Serialization.Clear();
            Assert.AreEqual(harness.Serialization.Count, 0);

            harness.Serialization.Add(new JsonContractSerializer());
            Assert.AreEqual(harness.Serialization.Count, 1);

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
            var harness = new ServiceHandlerContainer();

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
