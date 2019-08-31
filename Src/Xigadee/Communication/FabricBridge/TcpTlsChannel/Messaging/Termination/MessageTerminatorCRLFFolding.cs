namespace Xigadee
{
    /// <summary>
    /// This is the default terminator for the MessageCRLFFragment message class.
    /// </summary>
    public class MessageTerminatorCRLFFolding : MessageGenericTerminatorBase<CRLFMatchCollectionState>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageTerminatorCRLFFolding()
            : base()
        {
            Initialized = true;
            mState = new CRLFMatchCollectionState(AllowFolding);
#if (DEBUG)
            mState.DebugTrace = true;
#endif
        }
        #endregion

        #region AllowFolding
        /// <summary>
        /// This property specifies whether the message allows folding, that is a CRLF followed by a TAB or SPC character
        /// is not a termination. Otherwise the terminator will signal a match on CRLF.
        /// </summary>
        public virtual bool AllowFolding
        {
            get { return true; }
        }
        #endregion

        public override bool Initialized
        {
            get
            {
                return base.Initialized;
            }
            protected set
            {
                base.Initialized = value;
            }
        }
    }
}
