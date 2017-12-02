using System;

namespace Xigadee
{
    /// <summary>
    /// This policy contains the settings for the Serialization Container.
    /// </summary>
    /// <seealso cref="Xigadee.PolicyBase" />
    public class SerializationPolicy: PolicyBase
    {
        /// <summary>
        /// Specifies whether the serialization container supports the object registry. 
        /// By default this is false to preserve legacy behaviour.
        /// </summary>
        public bool ObjectRegistrySupported { get; set; } = false;
        /// <summary>
        /// This is the maximum number of objects that the registry will support. Leave this null to not define a limit.
        /// </summary>
        public int? ObjectRegistryLimit { get; set; } = 10000;

        /// <summary>
        /// This is the maximum time that an object can survive in the object registry,
        /// </summary>
        public TimeSpan ObjectRegistryTimeToLive { get; set; } = TimeSpan.FromSeconds(15);
        /// <summary>
        /// Specifies that a warning should be issued to the data collector when an object times out.  
        /// </summary>
        public bool ObjectRegistryWarningOnTimeout { get; set; } = true;
    }
}
