using System.IO;

namespace Xigadee
{
    public class MimeMessageFragment : MessageFragment<MimeMessageTerminator>, IMimeMessageInitialize
    {
        #region Declarations
        protected string mEncoding;
        #endregion

        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeMessageFragment()
            : base()
        {
            mEncoding = null;
        }
        #endregion

        public virtual Stream BodyStream
        {
            get
            {
                if (CanWrite || Initializing || !BodyLength.HasValue)
                    return null;
                return new MemoryStream(mBuffer, 0, (int)BodyLength.Value-2, false);
            }
        }


        public void Initialize(string boundary, string encoding)
        {
            mEncoding = encoding;
            Terminator.Initialize(boundary);
        }

    }
}
