using System;

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
