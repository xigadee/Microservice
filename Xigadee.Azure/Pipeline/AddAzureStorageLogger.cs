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
        public static MicroservicePipeline AddAzureStorageLogger(this MicroservicePipeline pipeline
            , string serviceName = null
            , string containerName = "log"
            , ResourceProfile resourceProfile = null
            , Action<AzureStorageLogger> onCreate = null)
        {
            return pipeline.AddAzureStorageLogger(pipeline.Configuration.LogStorageCredentials(), serviceName, containerName, resourceProfile);
        }

        public static MicroservicePipeline AddAzureStorageLogger(this MicroservicePipeline pipeline
            , StorageCredentials creds
            , string serviceName = null
            , string containerName = "log"
            , ResourceProfile resourceProfile = null
            , Action<AzureStorageLogger> onCreate = null)
        {
            var logger = new AzureStorageLogger(creds, serviceName ?? pipeline.Service.Name, containerName, resourceProfile);
            onCreate?.Invoke(logger);

            return pipeline.AddLogger(logger);
        }
    }
}
