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
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The security container class contains all the components to secure the incoming messaging for a Microservice, 
    /// and to ensure that incoming message requests have the correct permissions necessary to be processed.
    /// </summary>
    public partial class SecurityContainer: ServiceContainerBase<SecurityContainerStatistics, SecurityContainerPolicy>
        , ISecurityService, IRequireDataCollector, IServiceOriginator
    {
        #region Declarations
        /// <summary>
        /// These are the handlers used to encrpyt and decrypt blob payloads
        /// </summary>
        private Dictionary<string, IEncryptionHandler> mEncryptionHandlers;
        /// <summary>
        /// These are the handlers used to authenticate the incoming payloads, and
        /// sign the outgoing payloads.
        /// </summary>
        private Dictionary<string, IAuthenticationHandler> mAuthenticationHandlers;
        #endregion        
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="policy">The security policy.</param>
        public SecurityContainer(SecurityContainerPolicy policy) : base(policy)
        {
            mEncryptionHandlers = new Dictionary<string, IEncryptionHandler>();
            mAuthenticationHandlers = new Dictionary<string, IAuthenticationHandler>();
        }
        #endregion
        #region Collector
        /// <summary>
        /// This is the data collector used for logging.
        /// </summary>
        public IDataCollection Collector
        {
            get; set;
        }
        #endregion

        //Authentication
        #region HasAuthenticationHandler(string identifier)
        /// <summary>
        /// This method returns true if the authentication handler can be found.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        public bool HasAuthenticationHandler(string identifier)
        {
            return mAuthenticationHandlers.ContainsKey(identifier);
        }
        #endregion
        #region RegisterAuthenticationHandler(string identifier, IEncryptionHandler handler)
        /// <summary>
        /// This method registers an authentication handler with the collection.
        /// </summary>
        /// <param name="identifier">The identifier for the handler.</param>
        /// <param name="handler">The handler to register.</param>
        public void RegisterAuthenticationHandler(string identifier, IAuthenticationHandler handler)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentNullException("identifier");

            if (handler == null)
                throw new ArgumentNullException("handler");

            if (mAuthenticationHandlers.ContainsKey(identifier))
                throw new AuthenticationHandlerAlreadyExistsException(identifier);

            try
            {
                mAuthenticationHandlers.Add(identifier, handler);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"{nameof(RegisterAuthenticationHandler)} unexpected error.", ex);
                throw;
            }
        }
        #endregion

        //Encryption
        #region HasEncryptionHandler(string identifier)
        /// <summary>
        /// This method returns true if the encryption handler can be found.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        public bool HasEncryptionHandler(string identifier)
        {
            return mEncryptionHandlers.ContainsKey(identifier);
        }
        #endregion
        #region RegisterEncryptionHandler(string identifier, IEncryptionHandler handler)
        /// <summary>
        /// This method registers an encryption handler with the collection.
        /// </summary>
        /// <param name="identifier">The identifier for the handler.</param>
        /// <param name="handler">The handler to register.</param>
        public void RegisterEncryptionHandler(string identifier, IEncryptionHandler handler)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentNullException("identifier");

            if (handler == null)
                throw new ArgumentNullException("handler");

            if (mEncryptionHandlers.ContainsKey(identifier))
                throw new EncryptionHandlerAlreadyExistsException(identifier);

            try
            {
                mEncryptionHandlers.Add(identifier, handler);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"{nameof(RegisterEncryptionHandler)} unexpected error.", ex);
                throw;
            }
        }
        #endregion

        #region OriginatorId
        /// <summary>
        /// This is the system information.
        /// </summary>
        public MicroserviceId OriginatorId
        {
            get; set;
        } 
        #endregion

        protected override void StartInternal()
        {
            //Set the originators
            mAuthenticationHandlers.Values.ForEach((a) => a.OriginatorId = OriginatorId);
        }

        protected override void StopInternal()
        {

        }


        public byte[] Encrypt(EncryptionHandlerId handler, byte[] input)
        {
            EncryptionValidate(handler);

            return mEncryptionHandlers[handler.Id].Encrypt(input);
        }

        public byte[] Decrypt(EncryptionHandlerId handler, byte[] input)
        {
            EncryptionValidate(handler);

            return mEncryptionHandlers[handler.Id].Decrypt(input);
        }

        public bool EncryptionValidate(EncryptionHandlerId handler, bool throwErrors = true)
        {
            if (!mEncryptionHandlers.ContainsKey(handler.Id))
                if (throwErrors)
                    throw new EncryptionHandlerNotResolvedException(handler.Id);
            else
                    return false;

            return true;
        }
    }
}
