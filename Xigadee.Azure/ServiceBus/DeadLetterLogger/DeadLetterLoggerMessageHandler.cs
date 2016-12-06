#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
        /// <param name="encryption">Encryption to be used when logging dead letters</param>
        public DeadLetterLoggerMessageHandler(StorageCredentials credentials, string serviceName
            , TimeSpan? defaultTimeout = null
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , BlobRequestOptions options = null
            , OperationContext context = null
            , ISymmetricEncryption encryption = null)
        {
            mStorage = new StorageServiceBase(credentials, "DeadLetter", accessType, options, context, defaultTimeout: defaultTimeout, encryption:encryption);
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
