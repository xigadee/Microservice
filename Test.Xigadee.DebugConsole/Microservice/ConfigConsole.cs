using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This is the shared configuration class for both Microservices.
    /// </summary>
    internal class ConfigConsole:ConfigBase
    {

        public string ServiceBusConnection { get; set; }

        public StorageCredentials Storage { get; set; }

        public DocumentDbConnection DocDbCredentials { get; set; }

        public string DocDbDatabase { get; set; }

    }
}
