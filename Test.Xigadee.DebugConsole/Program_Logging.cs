using Xigadee;

namespace Test.Xigadee
{
    partial class Program
    {
        static void InitialiseEventSource(MicroserviceBase service)
        {
            //service.SharedServices.RegisterService<IRepositoryAsync<Guid,Customer>>()
            //service.RegisterEventSource(new DocumentDbEventSource(docDBserver, docDBkey, docDBdatabase, "Logging"));
            //service.RegisterEventSource(new EventHubEventSource("es1", connection, "es1"));
            //service.RegisterEventSource(new QueueEventSource("datapump", connection, "datapump"));
            service.RegisterEventSource(new AzureStorageEventSource(storageCreds, service.Statistics.Name, resourceProfile: mResourceBlob));
        }

        static void InitialiseLoggers(MicroserviceBase service)
        {
            //service.RegisterLogger(new DocumentDbLogger(
            //    docDBserver, docDBkey, docDBdatabase, "Logging"));
            service.RegisterLogger(new TraceEventLogger());
            service.RegisterLogger(new AzureStorageLogger(storageCreds, service.Statistics.Name, resourceProfile: mResourceBlob));
        }
    }
}
