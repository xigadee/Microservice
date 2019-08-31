using System;
using System.IO;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// The message contect class is used to receive data using specific byte markers.
    /// </summary>
    public class Message : MessageStreamBase, IMessageStream
    {
        #region Declarations
        /// <summary>
        /// This collection holds the fragments for the message.
        /// </summary>
        protected Dictionary<int, IMessage> mMessageParts = null;

        private int mFragmentReadPointer;
        private IMessage mFragmentReadCurrent;

        private object syncMessage = new object();
        private object syncAccess = new object();
        #endregion
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public Message()
            : base()
        {
            mFragmentReadPointer = 0;
            mFragmentReadCurrent = null;

            mMessageParts = new Dictionary<int, IMessage>();
        }
        #endregion


        #region Unused Stream methods and properties
        #region CanTimeout
        public virtual bool CanTimeout
        {
            get { return false; }
        }
        #endregion // CanTimeout
        #region ReadTimeout
        public virtual int ReadTimeout
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }
        #endregion // ReadTimeout
        #region WriteTimeout
        public virtual int WriteTimeout
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }
        #endregion // WriteTimeout
        #region CanSeek
        public virtual bool CanSeek
        {
            get { return false; }
        }
        #endregion // CanSeek
        #region Seek(long offset, SeekOrigin origin)
        public virtual long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException("Seek is not implemented.");
        }
        #endregion // Seek(long offset, SeekOrigin origin)
        #region SetLength(long value)
        public virtual void SetLength(long value)
        {
            throw new NotSupportedException("SetLength is not supported.");
        }
        #endregion // SetLength(long value)
        #endregion // Unused Stream methods and properties

        #region CanRead
        /// <summary>
        /// This method indicates whether data can be read from the message.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                if (Initializing)
                    return false;

                if (((int)Direction & (int)MessageDirection.Read) == 0)
                    return false;

                lock (syncAccess)
                {
                    if (FragmentCurrent == null)
                        return false;
                    return FragmentCurrent.CanRead || FragmentCanReadNext;
                }
            }
        }
        #endregion // CanRead
        #region Read(byte[] buffer, int offset, int count)
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
                throw new IOException("The stream does not currently support reading.");

            ByteBufferChecks(buffer, offset, count);

            int currentOffset = offset;
            int currentCount = count;
            int totalLength = 0;
            int length;
            do
            {
                if (FragmentCurrent == null || !FragmentCurrent.CanRead)
                    FragmentReadNext();

                length = FragmentCurrent.Read(buffer, currentOffset, currentCount);

                totalLength += length;
                currentOffset += length;
                currentCount -= length;

                Position += totalLength;

                //if (CompletionCheck())
                //    break;
            }
            while (currentCount > 0 && CanRead);

            return totalLength;
        }
        #endregion // Read(byte[] buffer, int offset, int count)

        #region CanWrite
        /// <summary>
        /// This boolean property indicates whether data can be written to the message.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                if (Initializing)
                    return false;

                if (((int)Direction & (int)MessageDirection.Write) == 0)
                    return false;

                lock (this)
                {
                    if (FragmentCurrent == null)
                        return false;
                    return FragmentCurrent.CanWrite;
                }
            }
        }
        #endregion // CanWrite
        #region Write(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method writes bytes to the message.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="offset">The buffer offset.</param>
        /// <param name="count">The number of bytes to read from the buffer.</param>
        /// <returns>Returns the number of bytes read from the buffer.</returns>
        public override int Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                throw new IOException("The stream does not currently support writing.");

            ByteBufferChecks(buffer, offset, count);
            lock (syncAccess)
            {
                int currentOffset = offset;
                int currentCount = count;
                int totalLength = 0;
                int length;

                do
                {
                    if (FragmentCurrent == null || !FragmentCurrent.CanWrite)
                        FragmentSetNext();

                    length = FragmentCurrent.Write(buffer, currentOffset, currentCount);
                    totalLength += length;
                    currentOffset += length;
                    currentCount -= length;

                    if (CompletionCheck())
                        break;

                }
                while (currentCount > 0);

                if (FragmentCurrent == null || !FragmentCurrent.CanWrite && !CompletionCheck())
                    FragmentSetNext();

                return totalLength;
            }
        }
        #endregion

        #region Length
        /// <summary>
        /// This method returns the length of the current message.
        /// </summary>
        public override long Length
        {
            get
            {
                lock (this)
                {
                    if (mMessageParts.Count == 0)
                        return 0;

                    long length = 0;
                    foreach (IMessage frag in Fragments)
                        length += frag.Length;

                    return length;
                }
            }
        }
        #endregion // Length

        #region Close()
        /// <summary>
        /// Close returns this object to the pool. You should not reference this object after you have closed it.
        /// </summary>
        public override void Close()
        {
        }
        #endregion // Close()
        #region CompletionCheck()
        /// <summary>
        /// This method should be called after each fragment is processed to
        /// </summary>
        protected virtual bool CompletionCheck()
        {
            return FragmentCurrent.IsTerminator;
        }
        #endregion // CompletionCheck()

        #region FragmentCanReadNext
        /// <summary>
        /// This method returns true
        /// </summary>
        protected virtual bool FragmentCanReadNext
        {
            get
            {
                return mFragmentReadPointer < mMessageParts.Count - 1;
            }
        }
        #endregion
        #region FragmentReadNext()
        /// <summary>
        /// This method moves to the next fragment.
        /// </summary>
        protected virtual void FragmentReadNext()
        {
            FragmentReadPointer++;
        }
        #endregion // FragmentReadNext()
        #region FragmentReadPointer
        /// <summary>
        /// This pointer sets the current fragment.
        /// </summary>
        protected virtual int FragmentReadPointer
        {
            get
            {
                return mFragmentReadPointer;
            }
            set
            {
                if (value >= mMessageParts.Count)
                    throw new MessageException("You cannot set the Read Pointer greater that the number of fragments.");
                mFragmentReadPointer = value;
                mFragmentReadCurrent = mMessageParts[value];
            }
        }
        #endregion // FragmentReadPointer

        #region FragmentSetNext()
        /// <summary>
        /// This method returns a new fragment object for the type specified.
        /// </summary>
        /// <returns>Returns the fragment object specified.</returns>
        protected virtual IMessage FragmentSetNext()
        {
            return FragmentSetNext(FragmentHeaderInitialType);
        }
        #endregion
        #region FragmentSetNext(Type fragmentType)
        /// <summary>
        /// This method returns a new fragment object for the type specified.
        /// </summary>
        /// <param name="fragmentType">The fragment type required.</param>
        protected virtual IMessage FragmentSetNext(Type fragmentType)
        {
            return FragmentSetNext(fragmentType, (MaxLength == -1 ? -1 : MaxLength - Length));
        }
        #endregion
        #region FragmentSetNext(Type fragmentType, int maxLength)
        /// <summary>
        /// This method returns a new fragment object for the type specified.
        /// </summary>
        /// <param name="fragmentType">The fragment type required.</param>
        /// <param name="maxLength">The maximum permitted length for the fragment.</param>
        protected virtual IMessage FragmentSetNext(Type fragmentType, long maxLength)
        {
            IMessage fragment = FragmentGet(fragmentType);
            //Set the maximum length of the fragment.
            fragment.Load(maxLength);
            FragmentAddInternal(fragment);
            return fragment;
        }
        #endregion
        #region FragmentAddInternal(IMessage fragment)
        /// <summary>
        /// This method adds the fragment to the internal collection.
        /// </summary>
        /// <param name="fragment">The fragment to add to the collection.</param>
        protected virtual void FragmentAddInternal(IMessage fragment)
        {
            mMessageParts.Add(mMessageParts.Count, fragment);
        }
        #endregion

        #region FragmentFirst
        /// <summary>
        /// This is the first fragment.
        /// </summary>
        protected virtual IMessage FragmentFirst
        {
            get
            {
                if (mMessageParts == null || mMessageParts.Count == 0)
                    return null;
                return mMessageParts[0];
            }
        }
        #endregion
        #region FragmentCurrent
        /// <summary>
        /// This is the last or current fragment.
        /// </summary>
        protected virtual IMessage FragmentCurrent
        {
            get
            {
                if (mFragmentReadCurrent != null)
                    return mFragmentReadCurrent;

                if (mMessageParts == null || mMessageParts.Count == 0)
                    return null;
                return mMessageParts[mMessageParts.Count - 1];
            }
        }
        #endregion

        #region FragmentInitialType
        /// <summary>
        /// This property is the initial incoming fragment type.
        /// </summary>
        protected virtual Type FragmentHeaderInitialType { get { throw new NotImplementedException("FragmentInitialType is not implemented."); } }
        #endregion // FragmentInitialType

        #region Fragments
        /// <summary>
        /// These are the fragments in the message.
        /// </summary>
        protected IEnumerable<IMessage> Fragments
        {
            get
            {
                foreach (IMessage item in mMessageParts.Values)
                    yield return item;
            }
        }
        #endregion

        #region Load(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method loads the message for read access.
        /// </summary>
        /// <param name="buffer">The buffer to load from.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The data length.</param>
        public override int Load(byte[] buffer, int offset, int count)
        {
            if (Loaded)
                throw new MessageLoadException("The message is already loaded.");
            if (Initializing)
                throw new MessageLoadException("The message is in the initialization phase.");

            ByteBufferChecks(buffer, offset, count);

            int currentOffset = offset;
            int currentCount = count;
            int totalLength = 0;
            int length;

            do
            {
                if (FragmentCurrent == null || !FragmentCurrent.CanWrite)
                    FragmentSetNext();

                length = FragmentCurrent.Load(buffer, currentOffset, currentCount);
                totalLength += length;
                currentOffset += length;
                currentCount -= length;

                if (CompletionCheck())
                    break;

            }
            while (currentCount > 0);

            Direction = MessageDirection.Read;
            Position = 0;

            return totalLength;
        }
        #endregion // Load(byte[] buffer, int offset, int count)
        #region Load(long maxSize)
        /// <summary>
        /// This load method creates an empty message ready for writing.
        /// </summary>
        /// <param name="maxSize">This is the maximum permitted size for the message.</param>
        public override void Load(long maxSize)
        {
            if (Initializing)
                throw new MessageLoadException("The message is in the initialization phase.");

            base.Load(maxSize);
            FragmentSetNext(FragmentHeaderInitialType);
        }
        #endregion // Load(long maxSize)

        #region DebugString
        /// <summary>
        /// This override combines the base DebugStrings in to a common string to aid with debugging.
        /// </summary>
        public override string DebugString
        {
            get
            {
                try
                {
                    string data = "";
                    foreach (IMessage frag in this.Fragments)
                    {
                        data += frag.DebugString;
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    return "Exception: " + ex.Message;
                }
            }
        }
        #endregion

        #region EndInitCustom()
        /// <summary>
        /// This method builds the fragment collection.
        /// </summary>
        protected override void EndInitCustom()
        {
            FragmentCollectionComplete();
        }
        #endregion
        #region FragmentCollectionComplete()
        /// <summary>
        /// This method is used to complete the header collection organization once the initialization phase has ended.
        /// </summary>
        protected virtual void FragmentCollectionComplete()
        {
            FragmentReadPointer = 0;
        }
        #endregion
        #region FragmentCollectionBuild()
        /// <summary>
        /// This method splits the instruction header in to its constituent parts.
        /// </summary>
        protected virtual void FragmentCollectionBuild()
        {
            FragmentCollectionBuild(false);
        }
        /// <summary>
        /// This method splits the instruction header in to its constituent parts.
        /// </summary>
        /// <param name="force">Set this parameter to true if you wish to force a rebuild.</param>
        protected virtual void FragmentCollectionBuild(bool force)
        {

        }
        #endregion

        #region FragmentGet(Type fragmentType)
        /// <summary>
        /// This method returns a new fragment object of the specified type. If the pool manager is not set
        /// this method will create a new fragment object.
        /// </summary>
        /// <param name="fragmentType">The fragment type.</param>
        /// <returns>Returns fragment object.</returns>
        protected virtual IMessage FragmentGet(Type fragmentType)
        {
            return (IMessage)Activator.CreateInstance(fragmentType);
        }
        #endregion  

    }
}
