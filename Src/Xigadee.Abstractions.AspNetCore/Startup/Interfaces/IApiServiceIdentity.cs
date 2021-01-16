using System;

namespace Xigadee
{
    /// <summary>
    /// This is the identity of the application.
    /// </summary>
    public interface IApiServiceIdentity
    {
        /// <summary>
        /// Gets the unique identifier for the instance.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        string MachineName { get; }
        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        string InstanceId { get; }

        /// <summary>
        /// The environment name.
        /// </summary>
        string EnvironmentName { get; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        string Application { get; }
        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        string Version { get; }
        /// <summary>
        /// Gets the start time for the application.
        /// </summary>
        DateTime Start { get; } 
        /// <summary>
        /// Gets the combined string.
        /// </summary>
        string Combined { get; }

        /// <summary>
        /// The JWT Audience that this service uses
        /// </summary>
        string JwtAudience { get; }
        /// <summary>
        /// Gets the domain for this Api.
        /// </summary>
        string Domain { get; }
    }
}
