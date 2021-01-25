using System;

namespace Xigadee
{
    /// <summary>
    /// This specifies whether the server will be run locally, or will be run connection to a remote Url
    /// </summary>
    public enum TestHostType
    {
        /// <summary>
        /// Local debug.
        /// </summary>
        Local,
        /// <summary>
        /// Test environment.
        /// </summary>
        Remote
    }
}
