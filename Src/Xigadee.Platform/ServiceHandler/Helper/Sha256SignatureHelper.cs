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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class verifies the signature.
    /// </summary>
    public class Sha256SignatureHelper: SignatureHelper<SHA256CryptoServiceProvider>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="salt">The case sensitive string that serves as the salt value for the signature.</param>
        public Sha256SignatureHelper(string salt) : base(salt)
        {
        }
        #endregion

        protected override SHA256CryptoServiceProvider CreateProvider()
        {
            return new SHA256CryptoServiceProvider();
        }
    }
}
