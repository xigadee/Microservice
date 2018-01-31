namespace Xigadee
{
    /// <summary>
    /// The compression id.
    /// </summary>
    /// <seealso cref="Xigadee.ServiceHandlerIdBase" />
    public class CompressionHandlerId: ServiceHandlerIdBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompressionHandlerId"/> class.
        /// </summary>
        /// <param name="id">The incoming identifier.</param>
        public CompressionHandlerId(string id) : base(id)
        {
        }


        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="id">The name of the resource profile.</param>
        public static implicit operator CompressionHandlerId(string id)
        {
            return new CompressionHandlerId(id);
        }


        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="handlerId">The handler id.</param>
        public static implicit operator string(CompressionHandlerId handlerId)
        {
            return handlerId.Id;
        }
    }
}
