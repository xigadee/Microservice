using System;
using System.Data.SqlClient;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to strip out the specific key parameters from the SqlContext.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    public interface ISqlEntityContextKey<K>
        where K : IEquatable<K>
    {
        /// <summary>
        /// This is the key.
        /// </summary>
        K Key { get; }

        /// <summary>
        /// This is the sql command to be executed.
        /// </summary>
        SqlCommand Command { get; }
    }
}
