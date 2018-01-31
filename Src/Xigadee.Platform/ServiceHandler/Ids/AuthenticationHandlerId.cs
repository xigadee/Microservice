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
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="id">The name of the resource profile.</param>
        public static implicit operator AuthenticationHandlerId(string id)
        {
            return new AuthenticationHandlerId(id);
        }


        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="handlerId">The handler id.</param>
        public static implicit operator string(AuthenticationHandlerId handlerId)
        {
            return handlerId.Id;
        }
    }
}
