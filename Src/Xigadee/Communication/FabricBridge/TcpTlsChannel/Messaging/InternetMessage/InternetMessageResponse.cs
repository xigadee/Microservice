using System;

namespace Xigadee
{
    public class InternetMessageResponse : InternetMessage
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetMessageResponse()
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
                return typeof(ResponseHeaderFragment);
            }
        }
        #endregion // FragmentInitialType
    }
}
