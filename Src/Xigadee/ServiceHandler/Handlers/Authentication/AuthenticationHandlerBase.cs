using System;

namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class for setting authentication
    /// </summary>
    public abstract class AuthenticationHandlerBase: IServiceHandlerAuthentication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationHandlerBase"/> class.
        /// </summary>
        /// <param name="id">This is the identifier.</param>
        /// <param name="name">The friendly name of the service.</param>
        protected AuthenticationHandlerBase(string id, string name)
        {
            Id = id ?? throw new ArgumentNullException("id");
            Name = name;
        }

        /// <summary>
        /// This property contains the Microservice identifiers used for claims source information.
        /// </summary>
        public MicroserviceId OriginatorId{get;set;}
        /// <summary>
        /// This method signs the outgoing payload.
        /// </summary>
        /// <param name="payload">The payload to sign.</param>
        public abstract void Sign(TransmissionPayload payload);
        /// <summary>
        /// This method verifies the incoming payload and sets the ClaimsPrincipal on the payload.
        /// </summary>
        /// <param name="payload">The payload to verify.</param>
        public abstract void Verify(TransmissionPayload payload);

        /// <summary>
        /// This is the data collector used for logging security events.
        /// </summary>
        public IDataCollection Collector
        {
            get; set;
        }
        /// <summary>
        /// This is the friendly name for the handler.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// This is the identifier name for the handler.
        /// </summary>
        public string Id { get; }
    }
}
