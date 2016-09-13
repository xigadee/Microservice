#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
        private RepositoryHolder<object, object> rqTemp;

        public PersistenceRepositoryHolder(RepositoryHolder<K, E> holder = null)
        {
            IsTimeout = false;
            Retry = 0;
            ShouldLogEventSource = true;

            if (holder != null)
            {
                Entity = holder.Entity;
                Key = holder.Key;
                KeyReference = holder.KeyReference;
                Settings = holder.Settings;
                ResponseCode = holder.ResponseCode;
                ResponseMessage = holder.ResponseMessage;
                TraceId = holder.TraceId;
            }

            Settings = Settings ?? new RepositorySettings();
        }

        public PersistenceRepositoryHolder(RepositoryHolder<object, object> rqTemp)
        {
            this.rqTemp = rqTemp;
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
            return $"{base.ToString()}|Timeout{Timeout}|IsTimeout={IsTimeout}|IsRetry={IsRetry}|Retry={Retry}|ShouldRetry={ShouldRetry}";
        }
    }
}
