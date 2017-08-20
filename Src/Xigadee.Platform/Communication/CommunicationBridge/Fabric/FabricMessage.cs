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

namespace Xigadee
{
    /// <summary>
    /// This is the base fabric message. It closely mirrors the BrokeredMessage of Service Bus to allow for simulations of functionality without the need 
    /// for a work service bus to test.
    /// </summary>
    public abstract class FabricMessage
    {
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

        public abstract void Signal(bool success);

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
