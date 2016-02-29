#region using
using System;
using System.Diagnostics;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the repository response metadata.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("ResponseCode={ResponseCode} ResponseMessage={ResponseMessage} IsSuccess={IsSuccess} IsFaulted={IsFaulted}")]
    public class RepositoryHolder<K, E> : RepositoryHolder<K>
    {
        #region Constructor
        /// <summary>
        /// This is the single constructor.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyReference"></param>
        /// <param name="entity"></param>
        /// <param name="responseCode"></param>
        /// <param name="responseMessage"></param>
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

        public virtual E Entity { get; set; }

        public override string ToString()
        {
            return string.Format("{0}|Entity={1}", base.ToString(), Entity);
        }
    }

    public class RepositoryHolder<K>:RepositoryHolder
    {
        public virtual K Key { get; set; }

        public override string ToString()
        {
            return string.Format("{0}|Key={1}", base.ToString(), Key);
        }
    }

    public class RepositoryHolder
    {
        public virtual Tuple<string,string> KeyReference { get; set; }

        public virtual RepositorySettings Settings { get; set; }

        public virtual int ResponseCode { get; set; }

        public virtual string ResponseMessage { get; set; }

        public virtual bool IsSuccess { get { return ResponseCode >= 200 && ResponseCode <= 299; } }

        public virtual bool IsCached { get;set; }

        public virtual bool IsFaulted { get { return ResponseCode >= 500 && ResponseCode <= 599; } }    

        public virtual string TraceId { get; set; }

        public override string ToString()
        {
            return string.Format("KeyReference={0}|ResponseCode={1}|ResponseMessage={2}|TraceId={3}", KeyReference, ResponseCode, ResponseMessage, TraceId);
        }
    }
}
