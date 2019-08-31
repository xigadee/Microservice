using System;
using System.Text;
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// The MessageStreamBase is for messages that implement a streaming functionality 
    /// </summary>
    public class MessageStreamBase : MessageBase, IMessage, IMessageStreamLoad
    {
        #region Declarations
        /// <summary>
        /// This protected variable defines whether the message can be read.
        /// </summary>
        private bool mCanRead;
        /// <summary>
        /// This protected variable defines whether the message can be written to.
        /// </summary>
        private bool mCanWrite;
        #endregion
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageStreamBase()
            : base()
        {
        }
        #endregion

        #region Direction
        /// <summary>
        /// This is the message direction
        /// </summary>
        public MessageDirection Direction
        {
            get;// { return mDirection; }
            protected set;// { mDirection = value; }
        }
        #endregion // Direction

        #region Position
        /// <summary>
        /// This is the data position for the stream.
        /// </summary>
        public virtual long Position
        {
            get;
            set;
        }
        #endregion
        #region Length
        /// <summary>
        /// This method returns the length of the current message.
        /// </summary>
        public virtual long Length
        {
            get;
            protected set;
        }
        #endregion // Length
        #region BodyLength
        /// <summary>
        /// This is the length of the body. This may be less than the length 
        /// if there are termination bytes at the end of the message
        /// </summary>
        public virtual long? BodyLength { get; protected set; }
        #endregion // BodyLength
        #region MaxLength
        /// <summary>
        /// This is the maximum length for the message.
        /// </summary>
        public virtual long MaxLength
        {
            get;// { return mMaxLength; }
            protected set;// { mMaxLength = value; }
        }
        #endregion // MaxLength

        #region CanRead
        /// <summary>
        /// Then this value is set to true the message stream can be read from.
        /// </summary>
        public virtual bool CanRead
        {
            get
            {
                if (((int)Direction & (int)MessageDirection.Read) == 0)
                    return false;

                return mCanRead && Position < Length; ;
            }
            protected set
            {
                mCanRead = value;
            }
        }
        #endregion // CanRead
        #region Read(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method reads bytes from the buffer in to the message.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer offset.</param>
        /// <param name="count">The data length.</param>
        /// <returns>Returns the number of bytes that have been copied tothe buffer.</returns>
        public virtual int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException("Read is not implemented.");
        }
        #endregion // Read(byte[] buffer, int offset, int count)
        #region ReadByte()
        public virtual int ReadByte()
        {
            byte[] buffer = new byte[1];
            if (this.Read(buffer, 0, 1) == 0)
            {
                return -1;
            }
            return buffer[0];
        }
        #endregion // ReadByte()

        #region CanWrite
        /// <summary>
        /// When this property is set to true, the message stream can be written to.
        /// </summary>
        public virtual bool CanWrite
        {
            get
            {
                if (((int)Direction & (int)MessageDirection.Write) == 0)
                    return false;

                return (Length < MaxLength || MaxLength == -1) && mCanWrite;// &!CanLoad;
            }
            protected set
            {
                if (!value & mCanWrite )
                    WriteComplete();

                mCanWrite = value;
            }
        }
        #endregion // CanWrite
        #region Write(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method reads from the incoming buffer and writes it to the fragment.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer offset.</param>
        /// <param name="count">The data length.</param>
        /// <returns>Returns the number of bytes actually read from the buffer.</returns>
        public virtual int Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException("Write is not implemented.");
        }
        #endregion
        #region WriteByte(byte value)
        /// <summary>
        /// This method writes 1 byte to the message
        /// </summary>
        /// <param name="value">The byte value to write</param>
        /// <returns>Returns the number of bytes actually read by the message.</returns>
        public virtual int WriteByte(byte value)
        {
            byte[] buffer = new byte[] { value };
            if (this.Write(buffer, 0, 1) >= 0)
            return buffer[0];

            return -1;
        }
        #endregion // WriteByte(byte value)
        #region WriteComplete()
        /// <summary>
        /// This method is called when the write status is changed to false and was previously true.
        /// </summary>
        protected virtual void WriteComplete()
        {

        }
        #endregion // WriteComplete()

        #region Close()
        /// <summary>
        /// Close returns this object to the pool. You should not reference this object after you have closed it.
        /// </summary>
        public virtual void Close()
        {
        }
        #endregion // Close()
        #region Flush()
        /// <summary>
        /// Flush does nothing in the default message.
        /// </summary>
        public virtual void Flush()
        {
        }
        #endregion // Flush()

        #region DebugString
        /// <summary>
        /// This string provides a ASCII representation of the byte buffer.
        /// </summary>
        public virtual string DebugString
        {
            get
            {
                throw new NotImplementedException("DebugString is not implemented.");
            }
        }
        #endregion
        #region IsTerminator
        /// <summary>
        /// This method returns true if the fragment has completed and is exactly equal to the termination string.
        /// </summary>
        public virtual bool IsTerminator
        {
            get
            {
                throw new NotImplementedException("MessageStreamBase/IsTerminator is not implemented.");
            }
        }
        #endregion

        #region BufferSizeInitial
        /// <summary>
        /// This property determines in initial incoming buffer size.
        /// </summary>
        protected virtual int BufferSizeInitial
        {
            get
            {
                return 1024;
            }
        }
        #endregion // BufferSizeInitial
        #region BufferSizeGrow
        /// <summary>
        /// This property determines the incremental growth size when the buffer needs to grow.
        /// </summary>
        protected virtual int BufferSizeGrow
        {
            get
            {
                return 256;
            }
        }
        #endregion // BufferSizeGrow
        #region BufferSizePrefer
        /// <summary>
        /// This property determines the prefered size for the buffer. Should the buffer exceed this value, the
        /// byte array will be recycled once the object returns to the pool.
        /// </summary>
        protected virtual long BufferSizePrefer
        {
            get
            {
                return 4096;
            }
        }
        #endregion // BufferSizeGrow
        #region BufferSizeMax
        /// <summary>
        /// This property determines the incremental growth size when the buffer needs to grow.
        /// </summary>
        protected virtual long BufferSizeMax
        {
            get
            {
                return int.MaxValue;
            }
        }
        #endregion // BufferSizeGrow

        #region ByteBufferChecks(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method does a set of buffer checks for the incoming data.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        protected virtual void ByteBufferChecks(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", "Buffer is null");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "Offset is less than 0.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Count is less than 0.");
            }
            if ((buffer.Length - offset) < count)
            {
                throw new ArgumentException("Invalid length.");
            }
        }
        #endregion

        #region Load()
        /// <summary>
        /// This creates a message with the maximum permissible size.
        /// </summary>
        public virtual void Load()
        {
            Load(BufferSizeMax);
        }
        #endregion // Load()
        #region Load(long maxSize)
        /// <summary>
        /// This method initializes the message for writing to the maximum default size.
        /// </summary>
        /// <param name="maxSize">The amximum permissible size for the message base.</param>
        public virtual void Load(long maxSize)
        {
            MaxLength = maxSize;
            CanWrite = true;
            Direction = MessageDirection.Write;
        }
        #endregion

        #region Load(Stream data)
        /// <summary>
        /// This method loads the message from a stream.
        /// </summary>
        /// <param name="data">The data stream to read from.</param>
        /// <returns>Returns the number of bytes read from the stream.</returns>
        public virtual int Load(Stream data)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion // Load(Stream data)
        #region Load(byte[] buffer, int offset, int count)
        public virtual int Load(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion // Load(byte[] buffer, int offset, int count)

        #region Load(string data)
        /// <summary>
        /// This method initializes the fragment with the default string data.
        /// </summary>
        /// <param name="data">The string.</param>
        /// <returns></returns>
        public virtual int Load(string data)
        {
            return Load(data, Encoding.UTF8);
        }
        #endregion // Load(string data)
        #region Load(string data, Encoding encoding)
        public virtual int Load(string data, Encoding encoding)
        {
            if (data == null)
                throw new ArgumentNullException("data", "data is null.");
            if (encoding == null)
                throw new ArgumentNullException("encoding", "encoding is null.");

            byte[] buffer = encoding.GetBytes(data);
            return Load(buffer, 0, buffer.Length);
        }
        #endregion // Load(string data, Encoding encoding)

        #region Initialization
        #region BeginInit()
        /// <summary>
        /// This method begin the load process for the message. During the initialization phase, fragments can be
        /// added and removed from the collection. Initialization cannot be started once the message has completed
        /// loading.
        /// </summary>
        public virtual void BeginInit()
        {
            if (!SupportsInitialization)
                throw new NotSupportedException("Initialization is not supported.");
            if (Initializing)
                return;

            Initializing = true;
        }
        #endregion // BeginInit()
        #region EndInit()
        /// <summary>
        /// This method completes the initialization phase and completes the loading process.
        /// </summary>
        public virtual void EndInit()
        {

            EndInitCustom();
            Initializing = false;
            CanWrite = false;
            CanRead = true;
            Direction = MessageDirection.Read;
            Position = 0;
        }
        #endregion // EndInit()
        #region EndInitCustom()
        /// <summary>
        /// This method should be overriden to provide any custom initialization completion code.
        /// </summary>
        protected virtual void EndInitCustom()
        {
            
        }
        #endregion // EndInitInternal()

        #region Initializing
        /// <summary>
        /// This property indicates whether the message is in an initialization phase.
        /// </summary>
        public virtual bool Initializing
        {
            get;
            protected set;
        }
        #endregion // Initializing

        #region SupportsInitialization
        /// <summary>
        /// This property indicates whether the fragment supports initialization.
        /// </summary>
        public virtual bool SupportsInitialization { get { return true; } }
        #endregion // SupportsInitialization
        #endregion

        #region Message Members


        public virtual byte[] ToArray()
        {
            throw new NotImplementedException();
        }

        public virtual byte[] ToArray(bool copy)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
