using System;
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
                    .AddAzureStorageDataCollector()
                    ;

                mainMenu.AddMicroservicePipeline(pipeline1);
                mainMenu.AddMicroservicePipeline(pipeline2);

                mainMenu.Show();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
