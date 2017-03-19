# The 15 minute Microservice

Xigadee is designed to try and simplify much of the complexity of setting up a Microservice. This example gives a brief outline of how you can build a Microservice based application using the Xigadee framework.

'''
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
'''

@paulstancer 
