#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    public class PersistenceRepositoryHolder<K, E> : RepositoryHolder<K, E>
    {
        public PersistenceRepositoryHolder(RepositoryHolder<K, E> holder = null)
        {
            IsTimeout = false;
            Retry = 0;
            ShouldLogEventSource = true;

            if (holder != null)
            {
                this.Entity = holder.Entity;
                this.Key = holder.Key;
                this.KeyReference = holder.KeyReference;
                this.Settings = holder.Settings;
                this.ResponseCode = holder.ResponseCode;
                this.ResponseMessage = holder.ResponseMessage;
                this.TraceId = holder.TraceId;
            }
        }

        public TimeSpan? Timeout { get; set; }

        public bool IsTimeout { get; set; }

        /// <summary>
        /// This property is used to signal to the underlying method that the request should be tried again, but has not failed due to a timeout or error.
        /// </summary>
        public bool ShouldRetry { get; set; }

        public bool ShouldLogEventSource { get; set; }

        public bool IsRetry { get; set; }

        public int Retry { get; set; }

        public RepositoryHolder<K, E> ToRepositoryHolder()
        {
            return new RepositoryHolder<K, E>()
            {
                Entity = this.Entity,
                Key = this.Key,
                KeyReference = this.KeyReference,
                Settings = this.Settings,
                ResponseCode = this.ResponseCode,
                ResponseMessage = this.ResponseMessage
            };
        }

        public override string ToString()
        {
            return string.Format( "{0}|Timeout{1}|IsTimeout={2}|IsRetry={3}|Retry={4}", base.ToString(), Timeout, IsTimeout, IsRetry, Retry);
        }
    }
}
