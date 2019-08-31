namespace Xigadee
{
    /// <summary>
    /// This is the default terminator for the MessageCRLFFragment message class.
    /// </summary>
    public class MessageTerminatorCRLFNoFolding : MessageTerminatorCRLFFolding
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageTerminatorCRLFNoFolding()
            : base()
        {
        }
        #endregion

        #region AllowFolding
        /// <summary>
        /// This property specifies whether the message allows folding, that is a CRLF followed by a TAB or SPC character
        /// is not a termination. Otherwise the terminator will signal a match on CRLF.
        /// </summary>
        public override bool AllowFolding
        {
            get { return false; }
        }
        #endregion
    }
}
