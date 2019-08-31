using System;

namespace Xigadee
{
    /// <summary>
    /// This is the default message type for internet based messaging.
    /// </summary>
    public class InternetMessage : HeaderBodyMessage
    {
        #region Declarations
        /// <summary>
        /// This is the collection of header positions within the message. This is required because certain
        /// headers may appears multiple times.
        /// </summary>
        protected InternetInstructionFragmentBase mInstruction;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetMessage()
            : base()
        {
            mInstruction = null;
        }
        #endregion

        #region HeaderExtract(string headerName)
        /// <summary>
        /// This method extract the header from the InternetMessage.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public HeaderFragment HeaderExtract(string headerName)
        {
            HeaderCollectionBuild();

            if (!mHeaderCollection.ContainsKey(headerName.ToLowerInvariant()))
                return null;

            int[] pos = mHeaderCollection[headerName];
            return pos.Length == 0 ? (HeaderFragment)null : mMessageParts[pos[0]] as HeaderFragment;
        }
        #endregion // HeaderExtract(string headerName)

        #region BodyLength
        /// <summary>
        /// This property returns the body length of the message.
        /// </summary>
        public override long? BodyLength
        {
            get
            {
                long bodyLength = 0;
                if (mHeaderCollection.ContainsKey("content-length"))
                {
                    int[] ids = mHeaderCollection["content-length"];
                    HeaderFragment header = mMessageParts[ids[0]] as HeaderFragment;
                    if (long.TryParse(header.FieldData, out bodyLength))
                        return bodyLength;
                }

                return null;
            }
        }
        #endregion // BodyLength
        #region Instruction
        /// <summary>
        /// This fragment is the first fragment in the internet message that contains the message instruction.
        /// </summary>
        public virtual InternetInstructionFragmentBase Instruction
        {
            get
            {
                if (Initializing && mInstruction == null)
                {
                    mInstruction = Activator.CreateInstance(FragmentHeaderInitialType) as InternetInstructionFragmentBase;
                    mInstruction.BeginInit();
                }

                if (!Initializing && mInstruction == null)
                {
                    FragmentCollectionBuild();
                }

                return mInstruction;
            }
        }
        #endregion // Instruction

        #region FragmentBodyType
        /// <summary>
        /// This is the fragment body type for the message. By default this fragment will 
        /// just store the body as a binary blob. You can override this property to return a more
        /// specific fragment type.
        /// </summary>
        protected override Type FragmentBodyType
        {
            get
            {
                return typeof(InternetMessageFragmentBody);
            }
        }
        #endregion // FragmentInitialType
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
                mBody = FragmentSetNext(FragmentBodyType, bodyLength.Value);
                return mBody;
            }

            return FragmentSetNext(FragmentHeaderType);
        }
        #endregion // FragmentSetNext()
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

            long? bl = BodyLength;

            return !bl.HasValue || bl.Value==0;
        }
        #endregion // CompletionCheck()

        #region FragmentCollectionBuild(bool force)
        /// <summary>
        /// This method builds the fragment collection.
        /// </summary>
        /// <param name="force"></param>
        protected override void FragmentCollectionBuild(bool force)
        {
            if (mMessageParts != null && (mMessageParts.Count > 0))
            {
                mInstruction = mMessageParts[0] as InternetInstructionFragmentBase;
            }
        }
        #endregion // FragmentCollectionBuild(bool force)
        #region FragmentCollectionComplete()
        /// <summary>
        /// This method is used to complete the header collection organization once the initialization phase has ended.
        /// </summary>
        protected override void FragmentCollectionComplete()
        {
            //This method adds the instruction to the start of the fragment collection.
            FragmentAddInstructionToStart();
            //This method adds any body metadata to the headers if the body is present.
            FragmentAddBodyMetadata();

            //Add the termination header to the end of the fragment collection.
            MessageCRLFFragment terminator = Activator.CreateInstance<MessageCRLFFragment>();
            terminator.BeginInit();
            terminator.DataString = "\r\n";
            HeaderAdd(terminator);

            //Add the body to the end of the fragment collection.
            if (mBody != null)
                mMessageParts.Add(mMessageParts.Count, mBody);
            //Finally tidy up and complete the initialization for all the message fragments.
            foreach (IMessage frag in mMessageParts.Values)
                frag.EndInit();

            base.FragmentCollectionComplete();
        }
        #endregion // FragmentCollectionComplete()

        #region FragmentAddInstructionToStart()
        /// <summary>
        /// This method moves down any headers and inserts the instruction to the beginning of the 
        /// fragment collection.
        /// </summary>
        protected virtual void FragmentAddInstructionToStart()
        {
            int headerCount = mMessageParts.Count;
            //We are going to move up any headers to make room for the message instruction.
            if (headerCount > 0)
            {
                //Ok, we need to move the headers down one and set the instruction at the top.
                while (headerCount > 0)
                {
                    mMessageParts[headerCount] = mMessageParts[headerCount - 1];
                    headerCount--;
                }
            }
            //Ok, add the instruction.
            mMessageParts[0] = mInstruction;
        }
        #endregion // HeaderInstructionAddToStart()
        #region FragmentAddBodyMetadata()
        /// <summary>
        /// This method will be called whenever the message contains body data. This method add the 
        /// body metadata to the header tags.
        /// </summary>
        protected virtual int FragmentAddBodyMetadata()
        {
            if (mBody == null)
                return 0;

            InternetMessageFragmentBody body = mBody as InternetMessageFragmentBody;
            if (body == null)
                return 0;

            int headersAdded = 0;

            if (body.Length > 0)
            {
                HeaderAdd("Content-Length", body.Length.ToString());
                headersAdded++;
            }

            if (body.HasLastModified)
            {
                HeaderAdd("Last-Modified", body.LastModified);
                headersAdded++;
            }

            if (body.HasContentEncoding)
            {
                HeaderAdd("Content-Encoding", body.ContentEncoding);
                headersAdded++;
            }

            if (body.HasContentMD5)
            {
                HeaderAdd("Content-MD5", body.ContentMD5);
                headersAdded++;
            }

            if (body.HasContentType)
            {
                HeaderAdd("Content-Type", body.ContentType);
                headersAdded++;
            }

            if (body.HasContentRange)
            {
                HeaderAdd("Content-Range", body.ContentRange);
                headersAdded++;
            }

            if (body.HasContentLanguage)
            {
                HeaderAdd("Content-Language", body.ContentLanguage);
                headersAdded++;
            }

            if (body.HasETag)
            {
                HeaderAdd("ETag", body.ETag);
                headersAdded++;
            }

            if (body.HasExpires)
            {
                HeaderAdd("Expires", body.Expires);
                headersAdded++;
            }

            return headersAdded;
        }
        #endregion // HeaderBodyAddMetadata()
    }
}
