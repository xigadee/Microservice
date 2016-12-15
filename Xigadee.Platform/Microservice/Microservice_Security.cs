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

#region using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //Process
    public partial class Microservice
    {
        /// <summary>
        /// This method registers a symmetric encryption handler with the Security container.
        /// </summary>
        /// <param name="identifier">The identifier. This is used to identify the handler so that it can be assigned to multiple channels.</param>
        /// <param name="handler">The actual handler.</param>
        public void RegisterSymmetricEncryptionHandler(string identifier, IEncryptionHandler handler)
        {
            mSecurity.RegisterSymmetricEncryptionHandler(identifier, handler);
        }
    }
}
