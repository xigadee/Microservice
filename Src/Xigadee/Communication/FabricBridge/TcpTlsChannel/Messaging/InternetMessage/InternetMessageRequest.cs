using System;

namespace Xigadee
{
    public class InternetMessageRequest : InternetMessage
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetMessageRequest()
            : base()
        {
        }
        #endregion

        #region FragmentInitialType
        /// <summary>
        /// This is the fragment type for the outgoing message.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(RequestHeaderFragment);
            }
        }
        #endregion // FragmentInitialType



    }
}
