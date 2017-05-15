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
    public class InternetInstructionFragmentBase : MessageCRLFFragment
    {
        #region Declarations
        string mVerb;
        string mProtocol;
        string mVersion;
        string mInstruction;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetInstructionFragmentBase()
            : base()
        {
            mVerb = null;
            mProtocol = null;
            mInstruction = null;
            mVersion = null;
        }
        #endregion
        
        #region IsRequest
        /// <summary>
        /// This property determines whether the message is a request. This is used to determine the order of the 
        /// three parts of the instruction. Request = Verb-Instruction-Protocol, Response = Protocol-Verb-Intruction.
        /// </summary>
        protected virtual bool IsRequest
        {
            get
            {
                return true;
            }
        }
        #endregion // IsRequest

        #region Verb
        /// <summary>
        /// This the the verb such as GET, POST etc for a Request or 200, 403, 404 for a Response message.
        /// </summary>
        public virtual string Verb
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();
                return mVerb; 
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("Protocol cannot be set when the message is not initializing.");
                mVerb = value;
            }
        }
        #endregion // Verb

        #region Instruction
        /// <summary>
        /// This is the instruction, either a Uri query for a request or a message for a response.
        /// </summary>
        public virtual string Instruction
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();
                return mInstruction;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("Protocol cannot be set when the message is not initializing.");
                mInstruction = value;
            }
        }
        #endregion // Instruction

        #region Protocol
        /// <summary>
        /// This is the protocol for the message. This property will generally be overriden for a specific
        /// protocol.
        /// </summary>
        public virtual string Protocol
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();
                return mProtocol;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("Protocol cannot be set when the message is not initializing.");
                mProtocol = value;
            }
        }
        #endregion // Protocol

        #region Version
        /// <summary>
        /// This is the protocol for the message. This property will generally be overriden for a specific
        /// protocol.
        /// </summary>
        public virtual string Version
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();
                return mVersion;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("Protocol cannot be set when the message is not initializing.");
                mVersion = value;
            }
        }
        #endregion // Protocol

        #region MessagePartsBuild()
        /// <summary>
        /// This method splits the instruction header in to its constituent parts.
        /// </summary>
        /// <param name="force">Set this parameter to true if you wish to force a rebuild.</param>
        protected virtual void MessagePartsBuild()
        {
            if (mVerb != null && mInstruction != null && mProtocol!=null)
                return;

            string data = DataString;
            if (data == null)
            {
                mVerb = null;
                mInstruction = null;
                mProtocol = null;
                return;
            }

            string [] mFragmentParts = data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (mFragmentParts.Length != 3)
                throw new MessageException("Instruction does not have the required parts.");
            string protver;

            if (IsRequest)
            {
                mVerb = mFragmentParts[0].Trim(' ');
                mInstruction = mFragmentParts[1].Trim(' ');
                protver = mFragmentParts[2].TrimStart(' ').TrimEnd('\r', '\n');
            }
            else
            {
                protver = mFragmentParts[0].Trim(' ');
                mVerb = mFragmentParts[1].Trim(' ');
                mInstruction = mFragmentParts[2].TrimStart(' ').TrimEnd('\r', '\n');
            }

            ParseProtocolVersion(protver, ref mProtocol, ref mVersion);
        }

        protected virtual void ParseProtocolVersion(string protver, ref string protocol, ref string version)
        {
            if (!protver.Contains(@"/"))
            {
                protocol = null;
                version = null;
                return;
            }

            string[] parts = protver.Split(new char[] { '/' }, StringSplitOptions.None);
            protocol = parts[0];
            if (parts.Length > 1)
                version = parts[1];
            else
                version = null;
        }
        #endregion // FragmentCollectionBuild(bool force)

        #region EndInitCustom()
        /// <summary>
        /// This method is used to complete the header collection organization once the initialization phase has ended.
        /// </summary>
        protected override void EndInitCustom()
        {
            if (IsRequest)
                DataString = mVerb + " " + mInstruction + " " + mProtocol + @"/" + mVersion +"\r\n";
            else
                DataString = mProtocol + @"/" + mVersion + " " + mVerb + " " + mInstruction + "\r\n";

            mVerb = null;
            mInstruction = null;
            mProtocol = null;
            mVersion = null;

            base.EndInitCustom();
        }
        #endregion // FragmentCollectionComplete()
    }
}
