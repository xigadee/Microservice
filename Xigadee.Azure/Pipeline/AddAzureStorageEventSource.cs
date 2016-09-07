using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {

        public static MicroservicePipeline AddAzureStorageEventSource(
              this MicroservicePipeline pipeline
            , string serviceName = null
            , string containerName = "eventsource"
            , ResourceProfile resourceProfile = null
            , Action<AzureStorageEventSource> onCreate = null)
        {
            return pipeline.AddAzureStorageEventSource(pipeline.Configuration.LogStorageCredentials(), serviceName, containerName, resourceProfile, onCreate);
        }

        public static MicroservicePipeline AddAzureStorageEventSource(
              this MicroservicePipeline pipeline
            , StorageCredentials creds
            , string serviceName = null
            , string containerName = "eventsource"
            , ResourceProfile resourceProfile = null
            , Action<AzureStorageEventSource> onCreate = null)
        {
            var component = new AzureStorageEventSource(creds, serviceName ?? pipeline.Service.Name, containerName, resourceProfile);

            onCreate?.Invoke(component);
                
            return pipeline.AddEventSource(component);
        }
    }
}
