using System;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This is base class for message termination. This class is used to provide custom termination logic for messages
    /// that use specific characters to signal completion when reading from a stream.
    /// </summary>
    public class MessageTerminatorBase : IMessageTermination
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageTerminatorBase()
            : base()
        {
            CurrentSection = -1;
            Length = 0;
            Initialized = false;
            CarryOver = 0;
            IsTerminator = false;
            Matched = false;
            BoundaryPartCondition = false;
        }
        #endregion


        #region CurrentSection
        /// <summary>
        /// This property identifies the current match section.
        /// </summary>
        public virtual int CurrentSection
        {
            get;
            protected set;
        }
        #endregion
        #region Length
        /// <summary>
        /// This property shows the number of bytes read in the stream.
        /// </summary>
        public virtual int Length
        {
            get;
            protected set;
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
        #region CarryOver
        /// <summary>
        /// This is carry-over value. This value is used to set the current position in the terminator. This is needed because
        /// terminators can be split across multiple writes. 
        /// </summary>
        public virtual int CarryOver
        {
            get;
            protected set;
        }
        #endregion // CarryOver
        #region BoundaryPartCondition
        /// <summary>
        /// This is carry-over value. This value is used to set the current position in the terminator. This is needed because
        /// terminators can be split across multiple writes. 
        /// </summary>
        public virtual bool BoundaryPartCondition
        {
            get;
            protected set;
        }
        #endregion // CarryOver
        #region IsTerminator
        /// <summary>
        /// This property is set to true whenever the byte array has reached predetermined termination characteristics.
        /// These characteristics will differ depending on the termination type. 
        /// </summary>
        public virtual bool IsTerminator
        {
            get;
            protected set;
        }
        #endregion // IsTerminator
        #region ValidateBoundaryPartCondition
        /// <summary>
        /// ValidateBoundaryPartCondition is used to check that the boundary condition is valid based on the current buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="carryOver">The carry over bytes.</param>
        /// <param name="bytesRead">The bytes read.</param>
        /// <returns></returns>
        protected virtual bool ValidateBoundaryPartCondition(byte[] buffer, int offset, int count, int carryOver, int bytesRead)
        {
            throw new NotImplementedException("MessageTerminatorBase/ValidateBoundaryPartCondition is not implemented.");
        }
        #endregion // ValidateBoundaryPartCondition

        #region Matched
        /// <summary>
        /// This property indicates when the termination character has been matched.
        /// </summary>
        public bool Matched
        {
            get;
            protected set;
        }
        #endregion // Matched

        #region TerminatorHolderGetFirst()
        /// <summary>
        /// This method returns the first terminator. This will reset the terminator count to 0 and then call TerminatorHolderGetCurrent().
        /// </summary>
        /// <returns>Returns a TerminatorHolder structure.</returns>
        protected virtual TerminatorHolder? TerminatorHolderGetFirst()
        {
            CurrentSection = 0;
            return TerminatorHolderGetCurrent();
        }        
        #endregion
        #region TerminatorHolderGetCurrent()
        /// <summary>
        /// This method returns the current terminator. You should override this method.
        /// </summary>
        /// <returns>Returns a TerminatorHolder structure.</returns>
        protected virtual TerminatorHolder? TerminatorHolderGetCurrent()
        {
            throw new NotImplementedException("MessageTerminatorBase/TerminatorHolderGetCurrent is not implemented.");
        }
        #endregion
        #region TerminatorHolderGetNext()
        /// <summary>
        /// This method returns the next terminator in the chain. If there is only one terminator, this method
        /// should return null.
        /// </summary>
        /// <returns>Returns a TerminatorHolder structure or null.</returns>
        protected virtual TerminatorHolder? TerminatorHolderGetNext()
        {
            throw new NotImplementedException("MessageTerminatorBase/TerminatorHolderGetNext is not implemented.");
        }
        #endregion // TerminatorHolderGetNext()

        #region Match(byte[] buffer, int offset, int count, out int length)
        /// <summary>
        /// This method scans the incoming byte array and returns true when the specific termination characteristics have been met.
        /// </summary>
        /// <param name="buffer">The incoming buffer.</param>
        /// <param name="offset">The buffer offset.</param>
        /// <param name="count">The available bytes.</param>
        /// <param name="bytesToRead">Returns the numbers of bytes that should be read in to the fragment.</param>
        /// <returns>Returns true if the termination characteristics have been met.</returns>
        public virtual bool Match(byte[] buffer, int offset, int count, out int bytesToRead, out long? bodyLength)
        {
            bodyLength = null;
            if (!Initialized)
                throw new NotSupportedException("Match is not supported without Initialization.");

            bytesToRead = 0;
            //Ok, once we have matched we do not process any further data until reset is called.
            if (Matched)
                return true;

            try
            {
                //Ok, get the current terminator.
                //TerminatorHolder? currentTH = TerminatorHolderGetCurrent();

                int newCarryOver;
                //First check whether we are carrying over some partially matched bytes from the previous buffer chunk
                if (CarryOver > 0 || BoundaryPartCondition)
                {
                    int matchReadPartial;
                    bool matchBoundaryPartCondition;
                    if (MatchTerminator(buffer, offset, count, CarryOver, out newCarryOver, 
                        out matchReadPartial, out matchBoundaryPartCondition))
                    {
                        //Is this a full match or just a partial match until we reached the end of the buffer.
                        if (newCarryOver == 0 && !matchBoundaryPartCondition)
                        {
                            //Ok, we have a full match. Return and write the remaining terminator bytes matched.
                            bytesToRead = matchReadPartial;
                            Matched = true;
                            return true;
                        }
                        else
                        {
                            //OK, we reached the end of the buffer before we could complete the full match, so we will carry over
                            //until the next chunk.
                            bytesToRead = matchReadPartial;
                            CarryOver = newCarryOver;
                            BoundaryPartCondition = matchBoundaryPartCondition;
                            return false;
                        }
                    }

                    //OK, no terminator, just a partial match, so continue as normal and set the carry-over value to 0.
                    CarryOver = 0;
                }
                //There are three scenarios:

                //1.    the buffer does not contain any termination characters - in which case just
                //      write it to the internal buffer;
                //2.    the buffer contains a termination character in which case write it to the buffer
                //      up until termination;
                //3.    a termination sequence is split between the internal buffer and the incoming buffer;

                int pos = offset - 1;
                int matchReadMain = 0;
                newCarryOver = 0;
                do
                {
                    //Move to the next byte after the previous match, or the first byte.
                    pos++;
                    pos = IndexOf(buffer, pos, count - (pos - offset));
                    //Case 1 - shortcut out of here, there are no terminator characters in the string.
                    //So just write the remaining bytes to the internal buffer.
                    if (pos == -1)
                    {
                        bytesToRead = count;
                        return false;
                    }
                    //Ok, we have matched the first character of the terminator, now check the remaining bytes
                    bool matchBoundaryPartCondition;
                    if (MatchTerminator(buffer, pos, count - (pos - offset), 0, out newCarryOver, 
                        out matchReadMain, out matchBoundaryPartCondition))
                    {
                        if (newCarryOver > 0 || matchBoundaryPartCondition)
                        {
                            //Case 3, we have a partial match at the end of the buffer
                            CarryOver = newCarryOver;
                            //Write the remaining buffer to the internal buffer
                            bytesToRead = count;
                            BoundaryPartCondition = matchBoundaryPartCondition;
                            return false; 
                        }

                        //Let's finish up, we have a matching terminator in the buffer.
                        break;
                    }
                }
                while (true);

                //OK, Case 2. Let's write the data up to the termination.
                bytesToRead = pos - offset + matchReadMain;
                Matched = true;
                return true;
            }
            catch (Exception ex)
            {
                bytesToRead = 0;
                throw ex;
            }
            finally
            {
                Length += bytesToRead;
            }
        }
        #endregion

        #region IndexOf(byte[] buffer, int offset, int count, byte value)
        /// <summary>
        /// This method searches the buffer for the first instance of the value byte.
        /// </summary>
        /// <param name="buffer">The buffer to search.</param>
        /// <param name="offset">The buffer offset.</param>
        /// <param name="count">The number of bytes to search.</param>
        /// <param name="value">The value of the byte to search for.</param>
        /// <returns>The position of the byte in the buffer, or -1 if the byte cannot be found.</returns>
        protected int IndexOf(byte[] buffer, int offset, int count)
        {
            TerminatorHolder? currentTH = TerminatorHolderGetCurrent();

            if (!currentTH.HasValue)
                return -1;

            return buffer.FindFirstPosition(offset, count, k => k == currentTH.Value.Terminator[0]);
        }
        #endregion

        #region ByteMatchSequence
        private bool ByteMatchSequence(byte[] terminator, byte[] buffer, int offset, int count,
            int terminatorOffset, out int carryOver, out int bytesRead)
        {
            
            carryOver = 0;
            bytesRead = 0;

            while ((terminatorOffset + bytesRead) < terminator.Length)
            {
                if (bytesRead >= count)
                {
                    carryOver = terminatorOffset + bytesRead;
                    return true;
                }

                if (buffer[offset + bytesRead] != terminator[CarryOver + bytesRead])
                {
                    return false;
                }
                bytesRead++;
            }

            return true;
        }
        #endregion // ByteMatchSequence
        #region ByteMatchAny
        private bool ByteMatchAny(byte[] terminator, byte[] buffer, int offset, int count,
            out int bytesRead, bool matchPositive)
        {
            bytesRead = 0;

            while (bytesRead < count)
            {
                byte item = buffer[offset + bytesRead];

                if (matchPositive)
                {
                    if (!terminator.Any(f => f == item))
                        return false;
                }
                else
                {
                    return !(terminator.Any(f => f == item));
                }

                bytesRead++;
            }

            return true;
        }
        #endregion // ByteMatchSequence

        #region MatchTerminator
        /// <summary>
        /// This method matches a the teminator in the byte buffer provided.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="offset">The offset position in the byte buffer that the search should begin.</param>
        /// <param name="count">The number of bytes in the buffer to search.</param>
        /// <param name="terminatorOffset">The offset parameter within the terminator.</param>
        /// <param name="carryOver">The carry over value. This is needed when there is a partial match to
        /// the terminator at the end of the byte buffer, it will be passed as the terminatorOffset when the 
        /// next set of data is received from the data stream.</param>
        /// <param name="bytesRead">This is the number of bytes to be read from the buffer.</param>
        /// <returns>Returns true if there is a full match, or true along with a carryOver value greater than 
        /// 0 when there is a partial match at the end of the byte buffer.</returns>
        protected bool MatchTerminator(byte[] buffer, int offset, int count,
            int terminatorOffset, out int carryOver, out int bytesRead, out bool matchBoundaryPartCondition)
        {
            TerminatorHolder currentTH = TerminatorHolderGetCurrent().Value;
            carryOver = 0;
            bytesRead = 0;
            matchBoundaryPartCondition = false;
            bool result = false;

            try
            {
                switch (currentTH.MatchType)
                {
                    case TerminatorHolderMatchType.MatchSequence:
                        result = ByteMatchSequence(currentTH.Terminator, buffer, offset, count, terminatorOffset, out carryOver, out bytesRead);

                        if (result && carryOver == 0)
                        {
                            //OK, we have matched a full terminator sequence. 
                            //Now we need to check whether there are additional sequences to match.
                            TerminatorHolder? nextTH = TerminatorHolderGetNext();
                            if (nextTH.HasValue)
                            {
                                int newOffset = offset + bytesRead;
                                int newCount = count - bytesRead;

                                if (newCount == 0)
                                {
                                    if (ValidateBoundaryPartCondition(buffer, offset, count, carryOver, bytesRead))
                                    {
                                        matchBoundaryPartCondition = true;
                                        return result;
                                    }
                                    else
                                        matchBoundaryPartCondition = false;
                                }

                                int newCarryOver, newBytesRead;
                                //Ok, we have further sequences, so we need to pass recursively to this method.
                                result = MatchTerminator(buffer, newOffset, newCount, 0, out newCarryOver, out newBytesRead, out matchBoundaryPartCondition);

                                if (result)
                                {
                                    bytesRead += newBytesRead;
                                    if (newCarryOver > 0)
                                        carryOver += newCarryOver;
                                    else
                                        carryOver = 0;
                                }
                            }
                        }
                        break;
                    case TerminatorHolderMatchType.MatchAny:
                        result = ByteMatchAny(currentTH.Terminator, buffer, offset, count, out bytesRead, 
                            currentTH.ActionType!= TerminatorHolderActionType.Exception);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //bytesRead
                if (!result)
                    TerminatorHolderGetFirst();
            }

            //OK, we have a match.
            return result;
        }
        #endregion

        #region Enum --> TerminatorHolderMatchType
        /// <summary>
        /// This enumeration specifies the match type.
        /// </summary>
        protected enum TerminatorHolderMatchType
        {

            MatchAny,

            MatchSequence
        }
        #endregion // TerminatorHolderMatchType
        #region Enum --> TerminatorHolderActionType
        /// <summary>
        /// This enumeration specifies the action type.
        /// </summary>
        protected enum TerminatorHolderActionType
        {
            /// <summary>
            /// This option specifies that the incoming byte array should be matched in the same order as the terminator.
            /// </summary>
            Termination = 1,
            /// <summary>
            /// This option specifies that the incoming byte array can be matched with any character that appears in the array and
            /// that signals a continuation.
            /// </summary>
            Exception = 2,
        }
        #endregion // TerminatorHolderActionType

        #region Struct --> TerminatorHolder
        /// <summary>
        /// This structure is used to hold the matching array.
        /// </summary>
        protected struct TerminatorHolder
        {
            #region Static declarations
            /// <summary>
            /// This is the default CRLF match.
            /// </summary>
            public static readonly TerminatorHolder CRLF;
            /// <summary>
            /// This is the default optional whitespace.
            /// </summary>
            public static readonly TerminatorHolder LWSPEx;
            /// <summary>
            /// This is the static constructor.
            /// </summary>
            static TerminatorHolder()
            {
                CRLF = new TerminatorHolder(new byte[] { 13, 10 },
                    TerminatorHolderMatchType.MatchSequence, TerminatorHolderActionType.Termination);
                LWSPEx = new TerminatorHolder(new byte[] { 9, 32 },
                    TerminatorHolderMatchType.MatchAny, TerminatorHolderActionType.Exception);
            }
            #endregion

            #region Declarations
            private byte[] mTerminator;
            private TerminatorHolderMatchType mMatchType;
            private TerminatorHolderActionType mActionType;
            #endregion
            #region Constructor
            public TerminatorHolder(byte[] Terminator, TerminatorHolderMatchType MatchType, TerminatorHolderActionType ActionType)
            {
                mTerminator = Terminator;
                mMatchType = MatchType;
                mActionType = ActionType;
            }
            #endregion

            #region Terminator
            /// <summary>
            /// The terminator byte array.
            /// </summary>
            public byte[] Terminator { get { return mTerminator; } }
            #endregion // Terminator
            #region MatchType
            /// <summary>
            /// The match type.
            /// </summary>
            public TerminatorHolderMatchType MatchType { get { return mMatchType; } }
            #endregion
            #region ActionType
            /// <summary>
            /// The action type.
            /// </summary>
            public TerminatorHolderActionType ActionType { get { return mActionType; } }
            #endregion

        }
        #endregion
    }
}
