namespace Xigadee
{
    /// <summary>
    /// The authentication id
    /// </summary>
    /// <seealso cref="Xigadee.ServiceHandlerIdBase" />
    public class AuthenticationHandlerId:ServiceHandlerIdBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationHandlerId"/> class.
        /// </summary>
        /// <param name="id">The incoming identifier.</param>
        public AuthenticationHandlerId(string id):base(id)
        {
        }

        /// <summary>
        /// Processes the incoming identifier in to a standard format..
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The processed identifier.
        /// </returns>
        public override string ProcessIdentifier(string id)
        {
            return base.ProcessIdentifier(id.Trim().ToLowerInvariant());
        }
        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="id">The name of the resource profile.</param>
        public static implicit operator AuthenticationHandlerId(string id)
        {
            if (id == null)
                return null;

            return new AuthenticationHandlerId(id);
        }


        /// <summary>
        /// Implicitly converts a handler to a string.
        /// </summary>
        /// <param name="handlerId">The handler id.</param>
        public static implicit operator string(AuthenticationHandlerId handlerId)
        {
            if (handlerId == null)
                return null;

            return handlerId.Id;
        }
    }
}
