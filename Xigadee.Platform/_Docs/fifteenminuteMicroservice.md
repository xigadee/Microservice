<table>
<tr>
<td width="80%"><a href="../../README.md"><img src="../../docs/X2a.png" alt="Xigadee"></a></td>
<td width = "*" align="right"><img src="../../docs/smallWIP.jpg" alt="Sorry, I'm still working here" height="100"></td>
</tr>
</table>

# The 15 minute Microservice

Xigadee is designed to try and simplify much of the complexity of setting up a Microservice. 
This example gives a brief outline of how you can quickly build a Microservice based application using the Xigadee framework.

For this exmple, we'll construct an API based service that can persist an entity (i.e. Create/Read/Update/Delete) in memory through a simple set of RESTful API call.

First we are going to create a new console application.
```
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
```

<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="../../docs/hitachi.png" alt="Hitachi Consulting" height="50"/></a></td> 
  <td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="../../README.md">Home</a></td>
</tr></table>
