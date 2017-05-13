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
    public class MimeMatchCollectionState : MatchCollectionState<byte, byte>
    {
        #region Declarations
        private MatchSequenceTerminator<byte, byte> mBoundaryTerminator;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the extended constructor.
        /// </summary>
        /// <param name="boundary"></param>
        public MimeMatchCollectionState(string boundary)
        {
            BoundaryValidate(ASCIIEncoding.ASCII.GetBytes(boundary));
        }
        #endregion // Constructors

        #region BoundaryValidate(byte[] boundary)
        /// <summary>
        /// This method validates the boundary.
        /// </summary>
        /// <param name="boundary">The ascii byte collection.</param>
        protected virtual void BoundaryValidate(byte[] boundary)
        {
            if (boundary.Length > 69)
                throw new ArgumentOutOfRangeException("boundary", "boundary is over the maximum permitted length of 69 characters.");

            mBoundaryTerminator = new MatchSequenceTerminator<byte, byte>(boundary, false); ;
        }
        #endregion // BoundaryValidate(byte[] boundary)

        #region GetEnumerator()
        /// <summary>
        /// This method returns a new MimeMatchCollection enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<MatchTerminator<byte, byte>> GetEnumerator()
        {
            return new MimeMatchCollection(mBoundaryTerminator);
        }
        #endregion // GetEnumerator()

        #region Class --> MimeMatchCollection
        /// <summary>
        /// The class contains the match terminators.
        /// </summary>
        public class MimeMatchCollection : MatchCollection<byte, byte>
        {
            #region Static constructor and declarations
            private static readonly MatchSequenceTerminator<byte, byte> mTerminatorDashRequired;
            private static readonly MatchSequenceSkipOrFailTerminator<byte, byte> mTerminatorDashOptionalTerminator;
            private static readonly MatchSkipTerminator<byte, byte> mTerminatorWHTSPSkip;
            private static readonly MatchSequenceTerminator<byte, byte> mTerminatorCRLF;

            static MimeMatchCollection()
            {
                mTerminatorDashRequired = 
                    new MatchSequenceTerminator<byte, byte>(
                        new byte[] { (byte)'-', (byte)'-' }, true);

                mTerminatorDashOptionalTerminator = 
                    new MatchSequenceSkipOrFailTerminator<byte, byte>(
                        new byte[] { (byte)'-', (byte)'-' }, false, null, null);

                mTerminatorWHTSPSkip = 
                    new MatchSkipTerminator<byte, byte>(
                        new byte[] { 9, 32 }, true);

                mTerminatorCRLF = 
                    new MatchSequenceTerminator<byte, byte>(
                        new byte[] { 13, 10 }, false);
            }
            #endregion // Static constructor and declarations
            #region Declarations
            private MatchSequenceTerminator<byte, byte> mBoundaryTerminator;
            private bool disposed = false;
            #endregion // Declarations
            #region Internal Constructor
            /// <summary>
            /// This constructor initializes the collection with the boundary.
            /// </summary>
            /// <param name="boundary"></param>
            internal MimeMatchCollection(MatchSequenceTerminator<byte, byte> boundaryTerminator)
                : base(null)
            {
                mBoundaryTerminator = boundaryTerminator;
            }
            #endregion // Constrcutor
            #region Dispose(bool disposing)
            /// <summary>
            /// This method disposes the collection.
            /// </summary>
            /// <param name="disposing"></param>
            public override void Dispose(bool disposing)
            {
                if (!disposed && disposing)
                {
                    mBoundaryTerminator = null;
                    base.Dispose(disposing);
                }
                disposed = true;
            }
            #endregion // Dispose(bool disposing)

            #region this[int index]
            /// <summary>
            /// This method returns the specified item for the collection. You should override this indexer.
            /// </summary>
            /// <param name="index">The position index.</param>
            /// <returns></returns>
            public override MatchTerminator<byte, byte> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: // "--"(required)
                            return mTerminatorDashRequired;
                        case 1: // boundary
                            return mBoundaryTerminator;
                        case 2: // "--"(optional)
                            return mTerminatorDashOptionalTerminator;
                        case 3: // WHSP skip
                            return mTerminatorWHTSPSkip;
                        case 4: // CRLF
                            return mTerminatorCRLF;
                        default:
                            throw new ArgumentOutOfRangeException("");
                    }
                }
            }
            #endregion // this[int index]
            #region Count
            /// <summary>
            /// This property returns the number of items in the collection. You should override this property.
            /// </summary>
            public override int Count
            {
                get { return 5; }
            }
            #endregion // Count
        }
        #endregion // Class --> MimeMatchCollection
    }
}
