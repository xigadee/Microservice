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

using System;
using System.Collections.Generic;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This is the base fabric message. It closely mirrors the BrokeredMessage of Service Bus to allow for simulations of functionality without the need 
    /// for a work service bus to test.
    /// </summary>
    public class FabricMessage
    {
        /// <summary>
        /// This action is used to signal that the message can be released back to the listener.
        /// </summary>
        private Action<bool, Guid> mSignalRelease = null;

        public FabricMessage()
        {
            Id = Guid.NewGuid();
            Properties = new Dictionary<string, string>();
        }

        public FabricMessage(Guid Id, IEnumerable<(string key,string value)> properties, byte[] payload, int deliveryAttempts = 0)
        {

        }

        public Guid Id { get; }

        public virtual Dictionary<string, string> Properties { get; }

        public virtual byte[] Message { get; set; }

        public virtual int DeliveryAttempts { get; set; } = 0;

        public virtual void Signal(bool success)
        {
            //We only want to do this once.
            var release = Interlocked.Exchange<Action<bool, Guid>>(ref mSignalRelease, null);

            if (release != null)
                try
                {
                    release(success, Id);
                }
                catch (Exception ex)
                {
                }
        }

        internal void ReleaseSet(Action<bool, Guid> release)
        {
            mSignalRelease = release;
        }

        public virtual void Complete()
        {
            Signal(true);
        }

        public virtual void Abandon()
        {
            Signal(false);
        }
    }
}
