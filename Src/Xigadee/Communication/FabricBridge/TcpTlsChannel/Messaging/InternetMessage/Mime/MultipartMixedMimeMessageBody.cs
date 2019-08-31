using System;

namespace Xigadee
{
    public class MultipartMixedMimeMessageBody : MimeBodyMessage
    {
        #region Declarations
        /// <summary>
        /// This is the main body fragment.
        /// </summary>
        protected PreambleMimeMessageBodyFragment mPreambleBody;
        /// <summary>
        /// This is the main body fragment.
        /// </summary>
        protected MessageFragment mEpilogueBody;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MultipartMixedMimeMessageBody()
            : base()
        {
        }
        #endregion

        #region FragmentInitialType
        /// <summary>
        /// This is the fragment initial type.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(PreambleMimeMessageBodyFragment);
            }
        }
        #endregion // FragmentInitialType
        #region FragmentFinalType
        /// <summary>
        /// This is the final type for the Mime message
        /// </summary>
        protected virtual Type FragmentFinalType
        {
            get
            {
                return typeof(MessageFragment);
            }
        }
        #endregion // FragmentFinalType

        #region FragmentSetNext()
        /// <summary>
        /// This method sets the next fragment in the message.
        /// </summary>
        protected override IMessage FragmentSetNext()
        {
            if (FragmentFirst == null)
            {
                return FragmentSetNext(FragmentHeaderInitialType);
            }

            if ((FragmentCurrent is MimeMessage || FragmentCurrent is MimeMessageFragment)
                && !FragmentCurrent.IsTerminator)
            {
                return FragmentSetNext(typeof(MimeMessage));
            }

            return FragmentSetNext(FragmentFinalType);
        }
        #endregion // FragmentSetNext()
    }
}
