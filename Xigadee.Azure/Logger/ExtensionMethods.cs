using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class AzureLoggerExtensionMethods
    {
        public static AzureStorageLogger AddAzureStorageLogger(this MicroservicePipeline pipeline
            , string serviceName = null
            , string containerName = "log"
            , ResourceProfile resourceProfile = null)
        {
            return pipeline.AddAzureStorageLogger(pipeline.Configuration.LogStorageCredentials(), serviceName, containerName, resourceProfile);
        }

        public static AzureStorageLogger AddAzureStorageLogger(this MicroservicePipeline pipeline
            , StorageCredentials creds
            , string serviceName = null
            , string containerName = "log"
            , ResourceProfile resourceProfile = null)
        {
            return pipeline.AddLogger((c) => new AzureStorageLogger(creds, serviceName ?? pipeline.Service.Name, containerName, resourceProfile));
        }
    }
}
