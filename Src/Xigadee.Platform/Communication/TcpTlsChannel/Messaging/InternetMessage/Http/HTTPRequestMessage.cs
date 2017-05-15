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

namespace Xigadee
{
    /// <summary>
    /// This class encapsulates the read stream for a HTTP request message.
    /// </summary>
    public class HTTPRequestMessage: InternetMessage
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HTTPRequestMessage()
            : base()
        {
        }
        #endregion

        #region FragmentInitialType
        /// <summary>
        /// This method returns the initial fragment type for the class.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(HTTPRequestHeaderFragment);
            }
        }
        #endregion

#if (DEBUG)
        public override int Write(byte[] buffer, int offset, int count)
        {
            return base.Write(buffer, offset, count);
        }
#endif

        #region Host
        /// <summary>
        /// This is the header host.
        /// </summary>
        public string Host
        {
            get
            {
                return HeaderSingle("host");
            }
        }
        #endregion // Host

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

            if (FragmentCurrent.IsTerminator)
            {
                long? bodyLength = BodyLength;

                if (!bodyLength.HasValue)
                    throw new ArgumentOutOfRangeException();

                ContentType cType = ContentType;

                switch (cType.MediaType)
                {
                    case "multipart/form-data":
                        var messageBody = new MultipartFormDataMimeMessageBody();

                        ////Set the maximum length of the fragment.
                        messageBody.Initialize(cType.Parameter("boundary"), null);

                        messageBody.Load(bodyLength.Value);
                        FragmentAddInternal(messageBody);

                        mBody = messageBody;
                        break;
                    case "application/x-www-form-urlencoded":
                        mBody = FragmentSetNext(typeof(WWWFormUrlEncodedBodyFragment), bodyLength.Value);
                        break;
                    case "":
                    default:
                        mBody = FragmentSetNext(typeof(InternetMessageFragmentBody), bodyLength.Value);
                        break;
                }

                return mBody;
            }

            return FragmentSetNext(FragmentHeaderType);
        }
        #endregion

        #region ContentType
        /// <summary>
        /// This property returns the body length of the message.
        /// </summary>
        public virtual ContentType ContentType
        {
            get
            {
                if (mHeaderCollection.ContainsKey("content-type"))
                {
                    int[] ids = mHeaderCollection["content-type"];
                    HeaderFragment header = mMessageParts[ids[0]] as HeaderFragment;
                    return new ContentType(header.FieldData);
                }

                return null;
            }
        }
        #endregion
    }
}
