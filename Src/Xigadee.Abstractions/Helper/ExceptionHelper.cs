using System;
using System.Runtime.CompilerServices;

namespace Xigadee
{
    /// <summary>
    /// This class provides helper methods for the exception class.
    /// </summary>
    public static class ExceptionHelper
    {
        #region AppendInnerExceptions(this Exception exception, string message = null)
        /// <summary>
        /// Formats the exception message including all inner exceptions. Useful when we have embedded exception trees.
        /// </summary>
        /// <param name="exception">The exception to convert to a string.</param>
        /// <param name="message">The existing message or null.</param>
        /// <returns>Returns the full exception stack appended to the string.</returns>
        public static string AppendInnerExceptions(this Exception exception, string message = null)
        {
            message = string.IsNullOrWhiteSpace(message) ? exception.Message : $"{message} > {exception.Message}";

            if (exception.InnerException != null)
                return exception.InnerException.AppendInnerExceptions(message);

            return message;
        }
        #endregion

        /// <summary>
        /// This helper method returns a short name for the calling class and the current line number.
        /// </summary>
        /// <param name="caller">The calling class where the type is needed.</param>
        /// <param name="memberName">This is filled in by the compiler.</param>
        /// <param name="sourceLineNumber">This is populated by the compiler.</param>
        /// <returns>Returns the debug string.</returns>
        public static string DebugPos<C>(this C caller,  
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0) where C: class =>
            $"{caller.GetType().Name}/{memberName}@{sourceLineNumber}";
    }
}
