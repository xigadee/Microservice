namespace Xigadee
{
    public class HTTPResponseHeaderFragment : ResponseHeaderFragment
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HTTPResponseHeaderFragment()
            : base()
        {
        }
        #endregion


        //public override string Protocol
        //{
        //    get
        //    {
        //        return base.Protocol;
        //    }
        //    set
        //    {
        //        base.Protocol = "HTTP/1.1";
        //    }
        //}

        
        #region FragmentCollectionComplete()
        /// <summary>
        /// This method is used to complete the header collection organization once the initialization phase has ended.
        /// </summary>
        protected override void EndInitCustom()
        {
            Protocol = "HTTP/1.1";

            base.EndInitCustom();
        }
        #endregion // FragmentCollectionComplete()
    }
}
