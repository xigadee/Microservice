namespace Xigadee
{
    /// <summary>
    /// This header extension conforms to RFC2047 and allows folding.
    /// </summary>
    public class MimeHeaderFragment : HeaderFragment<MessageTerminatorCRLFFolding>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeHeaderFragment()
            : base()
        {
        }
        #endregion

    }
}
