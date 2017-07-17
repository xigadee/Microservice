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
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class is used to terminate a mime based message.
    /// </summary>
    public class MimeMessageTerminator : MessageGenericTerminatorBase<MimeMatchCollectionState>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeMessageTerminator()
            : base()
        {
        }
        #endregion

        #region Initialize(string boundary)
        /// <summary>
        /// This method initializes the message terminator with the boundary string.
        /// </summary>
        /// <param name="boundary">The boundary data within the mime seperator.</param>
        public virtual void Initialize(string boundary)
        {
            base.mState = new MimeMatchCollectionState(boundary);

            Initialized = true;
        }
        #endregion


         
    }
}
