namespace Xigadee
{
    /// <summary>
    /// The serialization id.
    /// </summary>
    /// <seealso cref="Xigadee.ServiceHandlerIdBase" />
    public class SerializationHandlerId: ServiceHandlerIdBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationHandlerId"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public SerializationHandlerId(string id) : base(id)
        {
        }

        /// <summary>
        /// Processes the incoming identifier and strips off any additional parameters.
        /// </summary>
        /// <param name="id">The incoming identifier.</param>
        /// <returns>
        /// The processed identifier.
        /// </returns>
        public override string ProcessIdentifier(string id)
        {
            var items = id.Split(';');

            var value = items[0].Trim().ToLowerInvariant();

            return value;
        }

        /// <summary>
        /// Implicitly converts a string in to an id.
        /// </summary>
        /// <param name="id">The name of the resource profile.</param>
        public static implicit operator SerializationHandlerId(string id)
        {
            return new SerializationHandlerId(id);
        }


        /// <summary>
        /// Implicitly converts a handler id to a string.
        /// </summary>
        /// <param name="handlerId">The handler id.</param>
        public static implicit operator string(SerializationHandlerId handlerId)
        {
            return handlerId.Id;
        }
    }
}
