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
    public class MultipartMixedMimeMessageBody : MimeBodyMessage
    {
        #region Declarations
        /// <summary>
        /// This is the main body fragment.
        /// </summary>
        protected PreambleMimeMessageBodyFragment mPreambleBody;
        /// <summary>
        /// This is the main body fragment.
        /// </summary>
        protected MessageFragment mEpilogueBody;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MultipartMixedMimeMessageBody()
            : base()
        {
        }
        #endregion

        #region FragmentInitialType
        /// <summary>
        /// This is the fragment initial type.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(PreambleMimeMessageBodyFragment);
            }
        }
        #endregion // FragmentInitialType
        #region FragmentFinalType
        /// <summary>
        /// This is the final type for the Mime message
        /// </summary>
        protected virtual Type FragmentFinalType
        {
            get
            {
                return typeof(MessageFragment);
            }
        }
        #endregion // FragmentFinalType

        #region FragmentSetNext()
        /// <summary>
        /// This method sets the next fragment in the message.
        /// </summary>
        protected override IMessage FragmentSetNext()
        {
            if (FragmentFirst == null)
            {
                return FragmentSetNext(FragmentHeaderInitialType);
            }

            if ((FragmentCurrent is MimeMessage || FragmentCurrent is MimeMessageFragment)
                && !FragmentCurrent.IsTerminator)
            {
                return FragmentSetNext(typeof(MimeMessage));
            }

            return FragmentSetNext(FragmentFinalType);
        }
        #endregion // FragmentSetNext()
    }
}
