using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Azure
{
    [TestClass]
    //[Ignore] // Integration / debug testing only 
    public class ConfigResolverTableStorageTests
    {
        //File: Configuration.typed.csv
        /*
        PartitionKey,RowKey,Timestamp,Value,Value @type
        config,my_key,2017-09-03T12:17:54.827Z,123456,Edm.String
        otherconfig, my_key,2017-09-03T12:53:12.309Z,ABCDEF,Edm.String
        */

        /// <summary>
        /// All hail the Microsoft test magic man!
        /// This class can be populated with values through Visual Studio menus -> [Test>Test Settings>Select Test Settings File] and then selecting a file with the extension .runsettings
        /// See here for details: https://msdn.microsoft.com/en-us/library/jj635153.aspx
        /// There is a default file default.runsettings that has a set of empty CI injection parameters specified for testing in this project, you can set your own Azure configuration parameters to this file.
        /// </summary>
        public TestContext TestContext
        {
            get; set;
        }

        [TestMethod]
        public void TableStorageValidate1()
        {
            var ms1 = new MicroservicePipeline();
            ms1
                .ConfigResolverSetTestContext(TestContext)
                .ConfigResolverSetTableStorage()
                .ConfigResolverSetTableStorage(AzureStorageExtensionMethods.AzureTableStorageConfigDefaultPriority - 1, partitionKey:"otherconfig")
                ;

            Assert.IsTrue(ms1.Configuration.PlatformOrConfigCache("my_key") == "123456");
        }


        [TestMethod]
        public void TableStorageValidate2()
        {
            var ms1 = new MicroservicePipeline();
            ms1
                .ConfigResolverSetTestContext(TestContext)
                .ConfigResolverSetTableStorage()
                .ConfigResolverSetTableStorage(AzureStorageExtensionMethods.AzureTableStorageConfigDefaultPriority + 1, partitionKey: "otherconfig")
                ;

            Assert.IsTrue(ms1.Configuration.PlatformOrConfigCache("my_key") == "ABCDEF");
        }
    }
}
