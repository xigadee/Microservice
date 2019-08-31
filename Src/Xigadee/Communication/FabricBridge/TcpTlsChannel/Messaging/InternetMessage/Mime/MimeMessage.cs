using System;
using System.Collections.Generic;

namespace Xigadee
{
    public class MimeMessage : HeaderBodyMessage, IMimeMessageInitialize
    {
        #region Declarations
        /// <summary>
        /// This is the primary boundary for the MIME message.
        /// </summary>
        protected string mBoundary;
        /// This is the encoding type passed in the load constructor.
        /// </summary>
        protected string mEncoding;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeMessage()
            : base()
        {
        }
        #endregion

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

            //if (FragmentCurrent is HeaderFragmentMultiPart)
            //{
            //    HeaderFragmentMultiPart header = FragmentCurrent as HeaderFragmentMultiPart;

            //    if (header.Field.ToLowerInvariant() == "content-type")
            //    {
            //        mBoundary = header.FieldData;
            //    }
            //}

            if (FragmentCurrent.IsTerminator)
            {
                ContentType cType = ContentType;
                string mediaType = cType==null?"":cType.MediaType;
                switch (mediaType)
                {
                    case "multipart/mixed":
                        InitializeFragment<MultipartMixedMimeMessageBody>(mBoundary, null);
                        break;
                    //case "multipart/alternative":
                    //    InitializeFragment<MultipartAlternativeMimeMessageBody>(cType);
                    //    break;
                    //case "multipart/related":
                    //    InitializeFragment<MultipartRelatedMimeMessageBody>(cType);
                    //    break;
                    default:
                        InitializeFragment<MimeMessageFragment>(mBoundary, null);
                        break;
                }

                return mBody;
            }

            return FragmentSetNext(FragmentHeaderType);
        }
        #endregion // FragmentSetNext()

        #region LoadFragment<FRAG>(ContentType cType)
        /// <summary>
        /// This generic method loads the particular fragment.
        /// </summary>
        /// <typeparam name="FRAG"></typeparam>
        /// <param name="cType"></param>
        protected virtual void InitializeFragment<FRAG>(string boundary, string encoding)
            where FRAG : IMimeMessageInitialize, IMessage
        {
            FRAG fragment = Activator.CreateInstance<FRAG>();

            //Set the maximum length of the fragment.
            fragment.Initialize(boundary, encoding);
            fragment.Load();
            FragmentAddInternal(fragment);
            mBody = fragment;
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
                    IMessageHeaderFragment header = mMessageParts[ids[0]] as IMessageHeaderFragment;
                    return new ContentType(header.FieldData);
                }

                return null;
            }
        }
        #endregion // BodyLength


        public virtual void Initialize(string boundary, string encoding)
        {
            mEncoding = encoding;
            mBoundary = boundary;
        }


        #region CompletionCheck()
        /// <summary>
        /// This method is used to check whether the message has completed.
        /// </summary>
        /// <returns></returns>
        protected override bool CompletionCheck()
        {
            if (mBody != null)
                return !mBody.CanWrite;

            if (!FragmentCurrent.IsTerminator)
                return false;

            HeaderCollectionBuild();

            return false;
        }
        #endregion // CompletionCheck()

        #region IsTerminator
        /// <summary>
        /// This method returns true if the fragment has completed and is exactly equal to the termination string.
        /// </summary>
        public override bool IsTerminator
        {
            get
            {
                return mBody.IsTerminator;
            }
        }
        #endregion

        #region FragmentHeaderInitialType
        /// <summary>
        /// This is the fragment type for the outgoing message.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(HeaderFragmentMultiPart);
            }
        }
        #endregion // FragmentInitialType

        #region FragmentHeaderType
        /// <summary>
        /// This method returns the generic fragment header type.
        /// </summary>
        protected override Type FragmentHeaderType
        {
            get
            {
                return typeof(HeaderFragmentMultiPart);
            }
        }
        #endregion // FragmentHeader

        public HeaderFragmentMultiPart HeaderExtract(string headerName)
        {
            HeaderCollectionBuild();

            if (!mHeaderCollection.ContainsKey(headerName.ToLowerInvariant()))
                return null;

            int[] pos = mHeaderCollection[headerName];
            return pos.Length == 0? (HeaderFragmentMultiPart)null : mMessageParts[pos[0]] as HeaderFragmentMultiPart;
        }

        private string[] reserved = new string[] { "content-type", "content-transfer-encoding", "content-id", "content-description" };
        private string[] reservedMime = new string[] { "mime-version", "content-type", "content-transfer-encoding", "content-id", "content-description" };

        public HeaderFragmentMultiPart HeaderContent
        {
            get
            {
                return HeaderExtract("content-type");
            }
        }

        public HeaderFragmentMultiPart HeaderEncoding
        {
            get
            {
                return HeaderExtract("content-transfer-encoding");
            }
        }

        public HeaderFragmentMultiPart HeaderID
        {
            get
            {
                return HeaderExtract("content-id");
            }
        }

        public HeaderFragmentMultiPart HeaderDisposition
        {
            get
            {
                return HeaderExtract("content-disposition");
            }
        }

        public HeaderFragmentMultiPart HeaderDescription
        {
            get
            {
                return HeaderExtract("content-description");
            }
        }

        public HeaderFragmentMultiPart MimeVersion
        {
            get
            {
                return HeaderExtract("mime-version");
            }
        }

        public IEnumerable<HeaderFragmentMultiPart> ExtensionFields
        {
            get
            {
                foreach (HeaderFragmentMultiPart message in Headers)
                {
                    string name = message.Field.ToLowerInvariant();
                    if (name.StartsWith("content-") && !reserved.Contains(r => r == name ))
                        yield return message;
                }
            }
        }

        public IEnumerable<HeaderFragmentMultiPart> Fields
        {
            get
            {
                foreach (HeaderFragmentMultiPart message in Headers)
                {
                    string name = message.Field.ToLowerInvariant();
                    if (!name.StartsWith("content-") && name!="mime-version")
                        yield return message;
                }
            }
        }


        public IEnumerable<HeaderFragmentMultiPart> Headers
        {
            get
            {
                foreach(IMessage message in mMessageParts.Values)
                    if (message is HeaderFragmentMultiPart)
                        yield return (HeaderFragmentMultiPart)message;
            }
        }
    }
}
