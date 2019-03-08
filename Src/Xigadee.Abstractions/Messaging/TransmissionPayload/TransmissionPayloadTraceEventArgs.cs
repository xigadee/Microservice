using System;
using System.Diagnostics;
namespace Xigadee
{
    /// <summary>
    /// This class holds the trace information for the payload.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    [DebuggerDisplay("{Debug}")]
    public class TransmissionPayloadTraceEventArgs: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmissionPayloadTraceEventArgs"/> class.
        /// </summary>
        /// <param name="start">The tick-count start.</param>
        /// <param name="message">The message.</param>
        /// <param name="source">The optional source parameter.</param>
        /// <param name="instance">The optional source instance name.</param>
        public TransmissionPayloadTraceEventArgs(int start, string message = null, string source = null, string instance = null)
        {
            Extent = ConversionHelper.CalculateDelta(Environment.TickCount, start);
            Source = source;
            Message = message;
            Instance = instance;
        }

        /// <summary>
        /// Gets the tick count delta from when the payload was created.
        /// </summary>
        public int Extent { get; }
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { get; set;}
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set;}
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// This is the Debug string for the class
        /// </summary>
        public string Debug => $"{Extent}ms {Source}{(string.IsNullOrEmpty(Instance) ? "" : $" {Instance}")}: {Message}";
    }
}
