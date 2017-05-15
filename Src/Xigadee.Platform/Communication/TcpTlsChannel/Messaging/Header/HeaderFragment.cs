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
    public class HeaderFragment : HeaderFragment<MessageTerminatorCRLFNoFolding>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HeaderFragment()
            : base()
        {
        }
        #endregion
    }


    public class HeaderFragment<TERM> : MessageCRLFFragment<TERM>, IMessageHeaderFragment
        where TERM: MessageTerminatorCRLFFolding
    {
        #region Declarations
        /// <summary>
        /// This property contains the field name.
        /// </summary>
        protected string mField;
        /// <summary>
        /// This property contains the field data.
        /// </summary>
        protected string mFieldData;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HeaderFragment()
            : base()
        {
            mField = null;
            mFieldData = null;
        }
        #endregion

        #region Field
        /// <summary>
        /// This property contains the field name.
        /// </summary>
        public virtual string Field
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();

                return mField;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("The Field cannot be set when the fragment is not initializing.");
                mField = value.TrimStart(new char[] { ' ' }).TrimEnd(new char[] { ' ', ':' });
            }
        }
        #endregion
        #region FieldData
        /// <summary>
        /// This property contains the field data.
        /// </summary>
        public virtual string FieldData
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();

                return mFieldData;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("The FieldData cannot be set when the fragment is not initializing.");
                mFieldData = value.TrimStart(new char[] { ':', ' ' }).TrimEnd(new char[] { '\r', '\n' });
            }
        }
        #endregion

        #region MessagePartsBuild()
        /// <summary>
        /// This method breaks the DataString in to its constituent parts.
        /// </summary>
        /// <param name="force"></param>
        protected virtual void MessagePartsBuild()
        {
            if (mField != null && mFieldData != null)
                return;

            string data = DataString;
            int colonPos = data.IndexOf(':');
            if (colonPos == -1)
            {
                mField = null;
                mFieldData = null;
                return;
            }

            mField = data.Substring(0, colonPos).Trim();
            mFieldData = data.Substring(colonPos).TrimStart(new char[] { ':', ' ' }).TrimEnd(new char[] { '\r', '\n' });
        }
        #endregion

        #region EndInitCustom()
        /// <summary>
        /// This override sets the DataString at the end of the message initialization.
        /// </summary>
        protected override void EndInitCustom()
        {
            DataString = mField + ": " + mFieldData + "\r\n";
            mField = null;
            mFieldData = null;
            base.EndInitCustom();
        }
        #endregion // EndInitCustom()
    }

}
