using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the response message format for extended error logging.
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// The error subcode.
        /// </summary>
        public int? Subcode { get; set; }
        /// <summary>
        /// The error subcode.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// This is the byte blob to return if we are unable to deserialize the message.
        /// </summary>
        public byte[] Blob { get; set; }
    }
}
