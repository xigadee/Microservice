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
    /// This class is the base class used for matching message fragment termination in byte based arrays.
    /// </summary>
    /// <typeparam name="STATE">The state used for matching termination.</typeparam>
    public class MessageGenericTerminatorBase<STATE> : IMessageTermination
        where STATE: MatchCollectionState<byte, byte>
    {
        #region Declarations
        /// <summary>
        /// This is the match state.
        /// </summary>
        protected MatchCollectionState<byte, byte> mState = null;
        #endregion
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageGenericTerminatorBase()
            : base()
        {
            Initialized = false;
        }
        #endregion

        #region Initialized
        /// <summary>
        /// This property identifies whether the terminator has been initialized with the boundary.
        /// </summary>
        public virtual bool Initialized
        {
            get;
            protected set;
        }
        #endregion

        #region IsTerminator
        /// <summary>
        /// Returns true if the fragment is a termination setting.
        /// </summary>
        public bool IsTerminator
        {
            get { return mState.IsTerminator; }
        }
        #endregion
        #region CarryOver
        /// <summary>
        /// The number of carry over bytes in the buffer.
        /// </summary>
        public int CarryOver
        {
            get 
            { 
                return mState.SlidingWindow.Count; 
            }
        }
        #endregion

        #region Match(byte[] buffer, int offset, int count, out long length, out long? bodyLength)
        /// <summary>
        /// This is the match method. This method maps the incoming buffer to the match parameters.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="offset">The byte offset/</param>
        /// <param name="count">The number of bytes that can be read.</param>
        /// <param name="length">The length of the bytes read.</param>
        /// <returns>Returns true if a match has been found.</returns>
        public virtual bool Match(byte[] buffer, int offset, int count, out int length, out long? bodyLength)
        {
            if (!Initialized)
                throw new NotSupportedException("Match is not supported without Initialization.");

            //Get the current start position so that we can calculate the offset later.
            int lengthStart = mState.Length;

            //OK, match against the specified buffer range.
            mState = buffer
                .Range(offset, count)
                .MatchCollection(mState);

            //OK, calculate the number of bytes scanned.
            length = mState.Length - lengthStart;

            //OK, do we have a match?
            if (mState.Status.HasValue && (mState.Status.Value & MatchTerminatorStatus.Success) > 0)
            {
                //Yes, so calculate the actual body length.
                bodyLength = mState.MatchPosition - mState.Start;
                return true;
            }

            //OK, we don't have a match.
            bodyLength = null;

            return false;
        }
        #endregion
    }
}
