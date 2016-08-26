using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class AzureEventSourceExtensionMethods
    {

        public static AzureStorageEventSource AddAzureStorageEventSource(
            this MicroservicePipeline pipeline
            , string serviceName = null
            , string containerName = "eventsource"
            , ResourceProfile resourceProfile = null)
        {
            return pipeline.AddAzureStorageEventSource(pipeline.Configuration.LogStorageCredentials(), serviceName, containerName, resourceProfile);
        }

        public static AzureStorageEventSource AddAzureStorageEventSource(
            this MicroservicePipeline pipeline, StorageCredentials creds
            , string serviceName = null
            , string containerName = "eventsource"
            , ResourceProfile resourceProfile = null)
        {
            return pipeline.AddEventSource((c) => new AzureStorageEventSource(creds, serviceName ?? pipeline.Service.Name, containerName, resourceProfile));
        }
    }
}
