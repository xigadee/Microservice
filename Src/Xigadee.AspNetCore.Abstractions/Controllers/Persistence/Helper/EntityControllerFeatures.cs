using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the features that are enabled on the controller.
    /// </summary>
    [Flags]
    public enum EntityControllerFeatures
    {
        /// <summary>
        /// All features are disabled
        /// </summary>
        None = 0,
        /// <summary>
        /// The controller supports entity create
        /// </summary>
        Create = 1,
        /// <summary>
        /// The controller supports entity read
        /// </summary>
        Read = 2,
        /// <summary>
        /// The controller supports entity read by reference
        /// </summary>
        ReadByReference = 4,
        /// <summary>
        /// The controller supports all entity read requests
        /// </summary>
        ReadAll = 6,
        /// <summary>
        /// The controller supports entity update
        /// </summary>
        Update = 8,

        /// <summary>
        /// The controller supports entity delete
        /// </summary>
        Delete = 16,
        /// <summary>
        /// The controller supports entity delete by reference
        /// </summary>
        DeleteByReference = 32,
        /// <summary>
        /// The controller supports all entity delete requests
        /// </summary>
        DeleteAll = 48,

        /// <summary>
        /// The controller supports entity delete
        /// </summary>
        Version = 64,
        /// <summary>
        /// The controller supports entity delete by reference
        /// </summary>
        VersionByReference = 128,
        /// <summary>
        /// The controller supports all entity delete requests
        /// </summary>
        VersionAll = 192,

        /// <summary>
        /// The controller supports field search 
        /// </summary>
        Search = 256,
        /// <summary>
        /// The controller supports entity search 
        /// </summary>
        SearchEntity = 512,
        /// <summary>
        /// The controller supports all entity search types 
        /// </summary>
        SearchAll = 768,

        /// <summary>
        /// The controller supports all entity actions
        /// </summary>
        All = 1023
    }
}
