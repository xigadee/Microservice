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

namespace Xigadee
{
    /// <summary>
    /// This interface is responsible for the Microservice security.
    /// </summary>
    public interface IMicroserviceSecurity
    {
        /// <summary>
        /// This method registers an encryption handler with the Security container, which can encrypt and decrypt a binary blob.
        /// </summary>
        /// <param name="identifier">The identifier. This is used to identify the handler so that it can be assigned to multiple channels.</param>
        /// <param name="handler">The actual handler.</param>
        void RegisterEncryptionHandler(string identifier, IEncryptionHandler handler);

        /// <summary>
        /// This method specifies whether the microservice has the encryption handler registered.
        /// </summary>
        /// <param name="identifier">The identifier for the handler.</param>
        /// <returns>Returns true if the handler is registered.</returns>
        bool HasEncryptionHandler(string identifier);
    }
}
