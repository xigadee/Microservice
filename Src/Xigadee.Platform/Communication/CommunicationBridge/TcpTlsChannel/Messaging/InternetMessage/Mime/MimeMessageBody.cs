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
    /// This is the base Mime Body message class.
    /// </summary>
    public class MimeBodyMessage : Message, IMimeMessageInitialize
    {
        #region Declarations
        /// <summary>
        /// This is the mime boundary used in the deliminator.
        /// </summary>
        protected string mBoundary;
        /// <summary>
        /// This is the encoding type passed in the load constructor.
        /// </summary>
        protected string mEncoding;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeBodyMessage()
            : base()
        {
            mBoundary = null;
            mEncoding = null;
        }
        #endregion

        #region Initialize(string deliminator, string encoding)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deliminator"></param>
        /// <param name="encoding"></param>
        public virtual void Initialize(string boundary, string encoding)
        {
            mEncoding = encoding;
            mBoundary = boundary;
        }
        #endregion // Initialize(string deliminator, string encoding)

        #region FragmentSetNext(Type fragmentType, int maxLength)
        /// <summary>
        /// This method returns a new fragment object for the type specified.
        /// </summary>
        /// <param name="fragmentType">The fragment type required.</param>
        /// <param name="maxLength">The maximum permitted length for the fragment.</param>
        protected override IMessage FragmentSetNext(Type fragmentType, long maxLength)
        {
            IMessage fragment = Activator.CreateInstance(fragmentType) as IMessage;
            //Set the maximum length of the fragment.
            if (fragment is IMimeMessageInitialize)
                ((IMimeMessageInitialize)fragment).Initialize(mBoundary, mEncoding);

            fragment.Load(maxLength);
            FragmentAddInternal(fragment);
            return fragment;
        }
        #endregion


        public IEnumerable<MimeMessage> DataParts
        {
            get
            {
                foreach (IMessage message in mMessageParts.Values)
                {
                    if (message is MimeMessage)
                        yield return (MimeMessage)message;
                }
            }
        }

    }
}
