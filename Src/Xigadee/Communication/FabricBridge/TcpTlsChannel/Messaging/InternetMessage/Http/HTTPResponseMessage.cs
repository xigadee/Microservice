using System;

namespace Xigadee
{
    public class HttpProtocolResponseMessage : InternetMessage
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HttpProtocolResponseMessage()
            : base()
        {
        }
        #endregion
        #region FragmentInitialType
        /// <summary>
        /// This method returns the initial fragment type for the class.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(HTTPResponseHeaderFragment);
            }
        }
        #endregion
    }
}
