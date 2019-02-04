using System;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by a generic persistence request holder.
    /// </summary>
    public interface IPersistenceRequestHolder
    {
        /// <summary>
        /// Gets the profile identifier.
        /// </summary>
        Guid ProfileId { get; }
        /// <summary>
        /// Gets the start tick count.
        /// </summary>
        int Start { get; }
        /// <summary>
        /// Gets the debug string for the request..
        /// </summary>
        string Debug { get; }

    }
}
