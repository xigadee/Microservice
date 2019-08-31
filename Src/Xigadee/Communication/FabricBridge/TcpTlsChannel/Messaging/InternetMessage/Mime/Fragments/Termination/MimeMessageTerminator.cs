namespace Xigadee
{
    /// <summary>
    /// This class is used to terminate a mime based message.
    /// </summary>
    public class MimeMessageTerminator : MessageGenericTerminatorBase<MimeMatchCollectionState>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeMessageTerminator()
            : base()
        {
        }
        #endregion

        #region Initialize(string boundary)
        /// <summary>
        /// This method initializes the message terminator with the boundary string.
        /// </summary>
        /// <param name="boundary">The boundary data within the mime seperator.</param>
        public virtual void Initialize(string boundary)
        {
            base.mState = new MimeMatchCollectionState(boundary);

            Initialized = true;
        }
        #endregion


         
    }
}
