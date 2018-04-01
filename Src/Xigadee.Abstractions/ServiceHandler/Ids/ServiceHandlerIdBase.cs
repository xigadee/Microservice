namespace Xigadee
{
    /// <summary>
    /// This is base class for security related handlers.
    /// </summary>
    public abstract class ServiceHandlerIdBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHandlerIdBase"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        protected ServiceHandlerIdBase(string id)
        {
            Raw = id;
            Id = ProcessIdentifier(id);
        }

        /// <summary>
        /// Processes the incoming identifier in to a standard format..
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The processed identifier.</returns>
        public virtual string ProcessIdentifier(string id)
        {
            return id;
        }

        /// <summary>
        /// Gets the raw incoming id.
        /// </summary>
        public string Raw { get; }

        /// <summary>
        /// Gets the processed identifier.
        /// </summary>
        public string Id { get; }
    }
}
