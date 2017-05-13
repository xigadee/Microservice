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
    /// This is the default CRLF fragment that does not support folding.
    /// </summary>
    public class MessageCRLFFragment: MessageCRLFFragment<MessageTerminatorCRLFNoFolding>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageCRLFFragment()
            : base()
        {
        }
        #endregion
    }

    /// <summary>
    /// This is the base generic CRLF fragment. You can set the folding characteristics by setting the 
    /// generic term parameter to either TerminatorCRLF or TerminatorCRLFNoFolding.
    /// </summary>
    /// <typeparam name="TERM">The terminator class.</typeparam>
    public class MessageCRLFFragment<TERM> : MessageFragment<TERM>
        where TERM: MessageTerminatorCRLFFolding
    {
        #region Declarations
        Encoding mDefaultEncoding;
        #endregion
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageCRLFFragment()
            : base()
        {
            mDefaultEncoding = null;
        }
        #endregion

        #region DataString
        /// <summary>
        /// This is the string representation of the data using the default encoding.
        /// </summary>
        public virtual string DataString
        {
            get
            {
                if (InternalBuffer == null)
                    return null;
                return DefaultEncoding.GetString(InternalBuffer, 0, (int)Length);
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("The DataString cannot be set when the fragment is not initializing.");
                InternalBuffer = DefaultEncoding.GetBytes(value);
            }
        }
        #endregion
        #region DefaultEncoding
        /// <summary>
        /// This is the default encoding for the message
        /// </summary>
        public virtual Encoding DefaultEncoding
        {
            get
            {
                if (mDefaultEncoding == null)
                    return Encoding.UTF8;
                return mDefaultEncoding;
            }
            protected set { mDefaultEncoding = value; }
        }
        #endregion

    }
}
