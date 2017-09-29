using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This static class contains a set of shared helper methods for messaging services.
    /// </summary>
    public static class MessagingHelper
    {
        #region ToSafeLower(string value)
        /// <summary>
        /// This method is to fix an issue on service bus where filters are case sensitive
        /// but our message types and action types are not.
        /// </summary>
        /// <param name="value">The incoming value.</param>
        /// <returns>The outgoing lowercase value.</returns>
        public static string ToSafeLower(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.ToLowerInvariant();
        }
        #endregion
    }
}
