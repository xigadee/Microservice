using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Xigadee
{
    /// <summary>
    /// This class is used to log any incoming dead letter messages in to the 
    /// blob storage store.
    /// </summary>
    public class DeadLetterLoggerMessageHandler: CommandBase
    {
        #region Declarations
        /// <summary>
        /// This is the azure storage wrapper.
        /// </summary>
        private StorageServiceBase mStorage;
        /// <summary>
        /// This is the channel name to log messages under.
        /// </summary>
        private string mServiceName;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="credentials">The azure storage credentails.</param>
        /// <param name="serviceName">The service name to log under.</param>
        /// <param name="defaultTimeout">The default timeout for each message.</param>
        /// <param name="accessType">The blog storage access type. By default this is set to private.</param>
        /// <param name="options">The blod request options.</param>
        /// <param name="context">The options context.</param>
        public DeadLetterLoggerMessageHandler(StorageCredentials credentials, string serviceName
            , TimeSpan? defaultTimeout = null
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , BlobRequestOptions options = null
            , OperationContext context = null)
        {
            mStorage = new StorageServiceBase(credentials, "DeadLetter", accessType, options, context, defaultTimeout: defaultTimeout);
            mServiceName = serviceName;
        } 
        #endregion

        protected override void CommandsRegister()
        {
            //base.CommandRegister();
            //CommandRegister(
        }

    }
}
