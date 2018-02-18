#region using
using System;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the logging level.
    /// </summary>
    [Flags]
    public enum ApiBoundaryLoggingFilterLevel
    {
        /// <summary>
        /// No logging of any information.
        /// </summary>
        None = 0,
        /// <summary>
        /// Include the exception event in the log
        /// </summary>
        Exception = 1,
        /// <summary>
        /// Include the  request event in the log
        /// </summary>
        Request = 2,
        /// <summary>
        /// Include the response event in the log
        /// </summary>
        Response = 4,
        /// <summary>
        /// Include the request content in the log
        /// </summary>
        RequestContent = 8,
        /// <summary>
        /// Include the response content in the log
        /// </summary>
        ResponseContent = 16,
        /// <summary>
        /// Include all data in the log
        /// </summary>
        All = 31
    }
}
