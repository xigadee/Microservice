#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using Microsoft.ServiceBus;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the Azure connection information.
    /// </summary>
    public class AzureConnection
    {
        /// <summary>
        /// This is the internal namespace manager.
        /// </summary>
        private NamespaceManager mNamespaceManager = null;
        /// <summary>
        /// This is the Azure connection string.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// This is the Azure connection name.
        /// </summary>
        public string ConnectionName { get; set; }
        /// <summary>
        /// This is the Azure namespace manager.
        /// </summary>
        public NamespaceManager NamespaceManager
        { 
            get 
            {
                return mNamespaceManager ?? (mNamespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString));
            }
        }
    }
}
