using System;
using System.Text;
using Xigadee;

namespace Test.Xigadee.Azure.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var pipeline1 = new MicroservicePipeline("AzureStorageTest1");
                var pipeline2 = new MicroservicePipeline("AzureStorageTest2");
                var mainMenu = new ConsoleMenu("Azure Storage DataCollector validation");

                pipeline1
                    .ConfigurationSetFromConsoleArgs(args)
                    .AddEncryptionHandlerAes("myid", Convert.FromBase64String("hNCV1t5sA/xQgDkHeuXYhrSu8kF72p9H436nQoLDC28="), keySize:256)
                    .AddAzureStorageDataCollector(handler:"myid")
                    ;

                
                mainMenu.AddMicroservicePipeline(pipeline1);
                mainMenu.AddMicroservicePipeline(pipeline2);
                mainMenu.AddOption("Aruba", (m,o) => pipeline1.Service.DataCollection.LogException(new Exception()));
                mainMenu.Show();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
