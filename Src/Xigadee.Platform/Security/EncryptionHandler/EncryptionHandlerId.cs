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
    /// This class encapsulates the encryption handler id.
    /// </summary>
    public class EncryptionHandlerId: SecurityHandlerIdBase
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="id">The id name.</param>
        public EncryptionHandlerId(string id):base(id)
        {
        }


        /// <summary>
        /// Implicitly converts a string in to a the encryption handler.
        /// </summary>
        /// <param name="id">The name of the resource id.</param>
        public static implicit operator EncryptionHandlerId(string id)
        {
            return new EncryptionHandlerId(id);
        }
    }
}
