namespace Xigadee
{
    /// <summary>
    /// This is base class for security related handlers.
    /// </summary>
    public abstract class SecurityHandlerIdBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityHandlerIdBase"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        protected SecurityHandlerIdBase(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Id { get; }
    }
}
