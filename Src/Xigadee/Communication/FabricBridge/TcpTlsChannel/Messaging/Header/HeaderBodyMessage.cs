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

namespace Xigadee﻿
{
    /// <summary>
    /// This message class is used for body type that have a set of headers and a single body that
    /// is defined by a specific delimiter. Examples of this are internet and mime based messages.
    /// </summary>
    public class HeaderBodyMessage: Message
    {
        #region Declarations
        /// <summary>
        /// This is the collection of header positions within the message. This is required because certain
        /// headers may appears multiple times.
        /// </summary>
        protected Dictionary<string, int[]> mHeaderCollection = null;
        protected object syncHeaderCol = new object();
        protected bool mHeaderCollectionAlreadyBuilt;
        /// <summary>
        /// This is the main body fragment.
        /// </summary>
        protected IMessage mBody;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HeaderBodyMessage()
            : base()
        {
            mBody = null;
            mHeaderCollectionAlreadyBuilt = false;
        }
        #endregion

        #region FragmentHeaderType
        /// <summary>
        /// This method returns the generic fragment header type.
        /// </summary>
        protected virtual Type FragmentHeaderType
        {
            get
            {
                return typeof(HeaderFragment);
            }
        }
        #endregion // FragmentHeader
        #region FragmentHeaderInitialType
        /// <summary>
        /// This is the fragment type for the outgoing message.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(HeaderFragment<MessageTerminatorCRLFFolding>);
            }
        }
        #endregion

        #region FragmentBodyType
        /// <summary>
        /// This is the fragment body type for the message. 
        /// </summary>
        protected virtual Type FragmentBodyType
        {
            get
            {
                throw new NotImplementedException("FragmentBodyType is not implemented.");
            }
        }
        #endregion // FragmentInitialType
        #region Body
        /// <summary>
        /// This message is the last fragment in the message that contains the message body. 
        /// If there is no body in the message this property will be blank.
        /// </summary>
        public virtual IMessage Body
        {
            get
            {
                if (!Initializing && mBody == null)
                {
                    FragmentCollectionBuild();
                }
                return mBody;
            }
            set
            {
                if (!Initializing)
                    throw new MessageException("The message body can only be set when the message is initializing.");
                mBody = value;
            }
        }
        #endregion // Body

        #region HeaderFragments()
        /// <summary>
        /// This method extracts the header value for the particular key.
        /// </summary>
        /// <returns>The collection of strings.</returns>
        public virtual IEnumerable<HeaderFragment> HeaderFragments()
        {
            if (!mHeaderCollectionAlreadyBuilt)
                yield break;

            foreach (string key in mHeaderCollection.Keys)
            {
                if (mHeaderCollection.ContainsKey(key))
                {
                    int[] values = mHeaderCollection[key];

                    if (values.Length == 0)
                        continue;

                    foreach (int i in values)
                    {
                        HeaderFragment frag = mMessageParts[i] as HeaderFragment;
                        if (frag != null)
                            yield return frag;
                    }
                }
            }
        }
        #endregion
        #region Headers(string keyValue)
        /// <summary>
        /// This method extracts the header value for the particular key.
        /// </summary>
        /// <param name="keyValue">The key to retrieve.</param>
        /// <returns>The collection of strings.</returns>
        public virtual IEnumerable<string> Headers(string keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException("keyValue", "keyValue cannot be null.");

            if (!mHeaderCollectionAlreadyBuilt)
                yield break;

            string key = keyValue.Trim().ToLower();

            if (mHeaderCollection.ContainsKey(key))
            {
                int[] values = mHeaderCollection[key];

                if (values.Length > 0)
                {
                    foreach (int i in values)
                    {
                        HeaderFragment frag = mMessageParts[i] as HeaderFragment;
                        if (frag != null)
                            yield return frag.FieldData;
                    }
                }
            }
        }
        #endregion

        #region HeaderExists(string key)
        /// <summary>
        /// This method returns true if the header is already in the message.
        /// </summary>
        /// <param name="key">The header key to check.</param>
        /// <returns>Returns true if the header exists.</returns>
        public virtual bool HeaderExists(string key)
        {
            if (!mHeaderCollectionAlreadyBuilt)
                throw new NotSupportedException("The header collection has not been built");

            return mHeaderCollection.ContainsKey(key);
        }
        #endregion // HeaderExists(string key)
        #region HeaderAdd
        public virtual void HeaderAdd(string Header, string Body)
        {
            HeaderAdd(FragmentHeaderType, Header, Body);
        }

        public virtual void HeaderAdd(Type headerType, string Header, string HeaderData)
        {
            if (!Initializing)
                throw new MessageException("The HeaderAdd instruction can only be called when the message is initializing.");

            HeaderFragment newFrag = Activator.CreateInstance(headerType) as HeaderFragment;
            newFrag.BeginInit();
            newFrag.Field = Header;
            newFrag.FieldData = HeaderData ?? "";
            HeaderAdd(newFrag);
        }

        public virtual void HeaderAdd(IMessage fragment)
        {
            if (!Initializing)
                throw new MessageException("The HeaderAdd instruction can only be called when the message is initializing.");

            FragmentAddInternal(fragment);
        }
        #endregion
        #region HeaderCollectionBuild()
        /// <summary>
        /// This method builds the header collection based on the header field. 
        /// The collection is built based on the lower case definition of the header.
        /// Multiple identical headers are supported, with the header position stored as an integer collection.
        /// </summary>
        protected virtual void HeaderCollectionBuild()
        {
            if (mHeaderCollectionAlreadyBuilt)
                return;

            lock (syncHeaderCol)
            {
                if (mHeaderCollectionAlreadyBuilt)
                    return;

                if (mHeaderCollection == null)
                    mHeaderCollection = new Dictionary<string, int[]>();

                int id = -1;
                foreach (IMessage fragment in mMessageParts.Values)
                {
                    id++;

                    if (!(fragment is IMessageHeaderFragment))
                        continue;

                    var header = fragment as IMessageHeaderFragment;

                    if (header == null || header.Field == null)
                        continue;

                    if (!mHeaderCollection.ContainsKey(header.Field.ToLower()))
                    {
                        mHeaderCollection.Add(header.Field.ToLower(), new int[] { id });
                    }
                    else
                    {
                        int[] fields = mHeaderCollection[header.Field.ToLower()];

                        if (Array.IndexOf(fields, id) > -1)
                            continue;

                        Array.Resize(ref fields, fields.Length + 1);

                        fields[fields.Length - 1] = id;
                        mHeaderCollection[header.Field.ToLower()] = fields;
                    }
                }
                mHeaderCollectionAlreadyBuilt = true;
            }
        }

        #endregion // BuildHeaderCollection()
        #region HeaderSingle(string keyValue)
        /// <summary>
        /// This is the header host.
        /// </summary>
        public virtual string HeaderSingle(string keyValue)
        {
            if (keyValue == null)
                throw new ArgumentNullException("keyValue", "keyValue cannot be null.");

            if (!mHeaderCollectionAlreadyBuilt)
                return null;

            string key = keyValue.Trim().ToLower();

            int[] values = mHeaderCollection[key];

            if (values.Length > 1)
                throw new ArgumentOutOfRangeException("keyValue", "There are multiple headers for " + keyValue);
            
            HeaderFragment frag = mMessageParts[values[0]] as HeaderFragment;
            if (frag == null)
                return null;
            return frag.FieldData;
        }
        #endregion
    }
}
