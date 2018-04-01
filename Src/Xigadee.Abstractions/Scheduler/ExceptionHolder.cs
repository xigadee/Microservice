using System;

namespace Xigadee
{
    /// <summary>
    /// This holder is used to store exception information. We use this as some exceptions cause a failure when serialized.
    /// </summary>
    public class ExceptionHolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHolder"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="lastTime">The last time.</param>
        public ExceptionHolder(Exception ex, DateTime? lastTime)
        {
            ExceptionMessage = ex?.Message;
            ExceptionType = ex?.GetType().FullName;
            ExceptionTime = lastTime ?? DateTime.UtcNow;
        }
        /// <summary>
        /// The thrown exception message.
        /// </summary>
        public string ExceptionMessage { get; }
        /// <summary>
        /// The thrown exception type.
        /// </summary>
        public string ExceptionType { get; }

        /// <summary>
        /// This is the last exception time.
        /// </summary>
        public DateTime? ExceptionTime { get; }
    }
}
