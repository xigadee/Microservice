using System;
using System.Diagnostics;
namespace Xigadee
{
    #region RepositoryHolder<K, E>
    /// <summary>
    /// This class holds the repository response metadata.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("ResponseCode={ResponseCode} ResponseMessage={ResponseMessage} IsSuccess={IsSuccess} IsFaulted={IsFaulted}")]
    public class RepositoryHolder<K, E> : RepositoryHolder<K>
    {
        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryHolder{K, E}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="keyReference">The key reference.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="responseCode">The response code.</param>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="settings">The settings.</param>
        public RepositoryHolder(K key = default(K),
            Tuple<string, string> keyReference = null,
            E entity = default(E),
            int responseCode = 0,
            string responseMessage = null,
            RepositorySettings settings = null)
        {
            Key = key;
            KeyReference = keyReference;
            Entity = entity;

            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            Settings = settings ?? new RepositorySettings();
        }
        #endregion

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        public virtual E Entity { get; set; }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}|Entity={1}", base.ToString(), Entity);
        }
    }
    #endregion
    #region RepositoryHolder<K>
    /// <summary>
    /// The generic repository holder class.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <seealso cref="Xigadee.RepositoryHolder{K}" />
    public class RepositoryHolder<K> : RepositoryHolder
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public virtual K Key { get; set; }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}|Key={1}", base.ToString(), Key);
        }
    }
    #endregion
    #region RepositoryHolder
    /// <summary>
    /// The Repository holder class.
    /// </summary>
    /// <seealso cref="Xigadee.RepositoryHolder{K}" />
    public class RepositoryHolder
    {
        /// <summary>
        /// Gets or sets the key reference.
        /// </summary>
        public virtual Tuple<string, string> KeyReference { get; set; }
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public virtual RepositorySettings Settings { get; set; }
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        public virtual int ResponseCode { get; set; }
        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        public virtual string ResponseMessage { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is a success.
        /// </summary>
        public virtual bool IsSuccess { get { return ResponseCode >= 200 && ResponseCode <= 299; } }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is cached.
        /// </summary>
        public virtual bool IsCached { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is faulted.
        /// </summary>
        public virtual bool IsFaulted { get { return ResponseCode >= 500 && ResponseCode <= 599; } }
        /// <summary>
        /// Gets or sets the trace identifier.
        /// </summary>
        public virtual string TraceId { get; set; }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return $"KeyReference={KeyReference}|ResponseCode={ResponseCode}|ResponseMessage={ResponseMessage}|TraceId={TraceId}|CorrelationId={Settings?.CorrelationId}";
        }
    } 
    #endregion
}
