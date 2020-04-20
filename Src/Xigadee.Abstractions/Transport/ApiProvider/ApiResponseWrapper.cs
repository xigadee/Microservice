using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This is the base response wrapper.
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {

    }

    /// <summary>
    /// This is the generic response that includes an entity of a specific type.
    /// </summary>
    /// <typeparam name="RS">The entity response object</typeparam>
    [DebuggerDisplay("{ResponseCode}-{ResponseMessage}")]
    public class ApiResponse<RS>
    {
        /// <summary>
        /// The underlying HTTP response code.
        /// </summary>
        public int ResponseCode { get; set; }
        /// <summary>
        /// This is the optional response message.
        /// </summary>
        public string ResponseMessage { get; set; }
        /// <summary>
        /// This is the error object. This can be set by setting an error serializer.
        /// </summary>
        public object ErrorObject { get; set; }
        /// <summary>
        /// Gets or sets the response entity.
        /// </summary>
        public RS Entity { get; set; }
        /// <summary>
        /// Specifies whether the response is a success.
        /// </summary>
        public bool IsSuccess => ResponseCode >= 200 & ResponseCode <= 299;
        /// <summary>
        /// Specifies whether the response is a failure.
        /// </summary>
        public bool IsFailure => ResponseCode >= 400 & ResponseCode <= 499;
        /// <summary>
        /// Specifies whether the response is an exception.
        /// </summary>
        public bool IsException => ResponseCode >= 500 & ResponseCode <= 599;

        /// <summary>
        /// This is the default error object type.
        /// </summary>
        public class ErrorObjectDefault
        {
            /// <summary>
            /// The type specified in the HTTP error.
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// The raw content blob.
            /// </summary>
            public byte[] Blob { get; set; }
        }
    }
}
