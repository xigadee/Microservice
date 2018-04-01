#region using
using System;
using System.Collections.Generic;
using System.Net.Http;
#endregion
namespace Xigadee
{
    #region ResponseHolderBase
    /// <summary>
    /// This is root response class.
    /// </summary>
    /// <seealso cref="Xigadee.IResponseHolder" />
    public class ResponseHolderBase : IResponseHolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseHolderBase"/> class.
        /// </summary>
        public ResponseHolderBase()
        {
            Fields = new Dictionary<string, string>();
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is successful.
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is timed out.
        /// </summary>
        public bool IsTimeout { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is a cache hit.
        /// </summary>
        public bool IsCacheHit { get; set; }
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public virtual int StatusCode { get; set; }
        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public virtual string StatusMessage { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the version identifier.
        /// </summary>
        public string VersionId { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Gets or sets any exception raised.
        /// </summary>
        public Exception Ex { get; set; }
        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        public Dictionary<string, string> Fields { get; set; }
    } 
    #endregion
    #region ResponseHolder
    /// <summary>
    /// The root response holder.
    /// </summary>
    /// <seealso cref="Xigadee.ResponseHolderBase" />
    public class ResponseHolder : ResponseHolderBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is throttled.
        /// </summary>
        public bool IsThrottled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether a wait was involved before it returned.
        /// </summary>
        public bool ThrottleHasWaited { get; set; }
        /// <summary>
        /// Gets or sets the resource charge.
        /// </summary>
        public double ResourceCharge { get; set; }
        /// <summary>
        /// Gets or sets the throttle suggested wait time.
        /// </summary>
        public TimeSpan? ThrottleSuggestedWait { get; set; }
        /// <summary>
        /// Gets or sets the http response.
        /// </summary>
        public HttpResponseMessage Response { get; set; }
        /// <summary>
        /// Gets the status code.
        /// </summary>
        public override int StatusCode { get { return (Response != null ? (int)Response.StatusCode : base.StatusCode); } }
        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        public string DocumentId { get; set; }
        /// <summary>
        /// Gets or sets the ETag identifier.
        /// </summary>
        public string ETag { get; set; }
        /// <summary>
        /// Gets or sets the continuation token.
        /// </summary>
        public string ContinuationToken { get; set; }
        /// <summary>
        /// Gets or sets the session token.
        /// </summary>
        public string SessionToken { get; set; }
    } 
    #endregion
    #region ResponseHolder<O>
    /// <summary>
    /// This is the generic response holder that contains the entity
    /// </summary>
    /// <typeparam name="O">The entity type.</typeparam>
    /// <seealso cref="Xigadee.ResponseHolderBase" />
    public class ResponseHolder<O> : ResponseHolder, IResponseHolder<O>
        where O : class
    {
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        public O Entity { get; set; }
    } 
    #endregion
}
