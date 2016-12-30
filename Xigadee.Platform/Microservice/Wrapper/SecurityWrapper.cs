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

namespace Xigadee
{
    /// <summary>
    /// This wrapper is used to reduce the number of security interfaces implemented by the Microservice.
    /// </summary>
    internal class SecurityWrapper: WrapperBase, IMicroserviceSecurity
    {
        /// <summary>
        /// This container is used to hold the security infrastructure for the Microservice.
        /// </summary>
        private SecurityContainer mSecurity;

        public SecurityWrapper(SecurityContainer security, Func<ServiceStatus> getStatus):base(getStatus)
        {
            mSecurity = security;
        }

        /// <summary>
        /// This method registers a symmetric encryption handler with the Security container.
        /// </summary>
        /// <param name="identifier">The identifier. This is used to identify the handler so that it can be assigned to multiple channels.</param>
        /// <param name="handler">The actual handler.</param>
        public void RegisterEncryptionHandler(string identifier, IEncryptionHandler handler)
        {
            mSecurity.RegisterEncryptionHandler(identifier, handler);
        }

        /// <summary>
        /// This method specifies whether the microservice has the encryption handler registered.
        /// </summary>
        /// <param name="identifier">The identifier for the handler.</param>
        /// <returns>Returns true if the handler is registered.</returns>
        public bool HasEncryptionHandler(string identifier)
        {
            return mSecurity.HasEncryptionHandler(identifier);
        }

        /// <summary>
        /// This method registers an encryption handler with the Security container, which can encrypt and decrypt a binary blob.
        /// </summary>
        /// <param name="identifier">The identifier. This is used to identify the handler so that it can be assigned to multiple channels.</param>
        /// <param name="handler">The actual handler.</param>
        public void RegisterAuthenticationHandler(string identifier, IAuthenticationHandler handler)
        {
            mSecurity.RegisterAuthenticationHandler(identifier, handler);
        }

        /// <summary>
        /// This method specifies whether the microservice has the encryption handler registered.
        /// </summary>
        /// <param name="identifier">The identifier for the handler.</param>
        /// <returns>Returns true if the handler is registered.</returns>
        public bool HasAuthenticationHandler(string identifier)
        {
            return mSecurity.HasAuthenticationHandler(identifier);
        }
    }
}
