namespace Xigadee
{
    public class MultipartFormDataMimeMessageBody : MultipartMixedMimeMessageBody
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MultipartFormDataMimeMessageBody()
            : base()
        {
        }
        #endregion

        public override int Write(byte[] buffer, int offset, int count)
        {
            return base.Write(buffer, offset, count);
        }



    }
}
