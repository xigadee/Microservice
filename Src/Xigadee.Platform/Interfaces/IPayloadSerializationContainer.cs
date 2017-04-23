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
namespace Xigadee
{
    /// <summary>
    /// This interface is used to expose the serialization container to applications 
    /// that require access to it.
    /// </summary>
    public interface IPayloadSerializationContainer
    {
        object PayloadDeserialize(TransmissionPayload message);

        object PayloadDeserialize(ServiceMessage message);

        object PayloadDeserialize(byte[] blob);

        P PayloadDeserialize<P>(TransmissionPayload payload);

        P PayloadDeserialize<P>(ServiceMessage payload);

        P PayloadDeserialize<P>(byte[] blob);

        byte[] PayloadSerialize(object payload);
    }

    /// <summary>
    /// This interface is for components that require payload serialization and deserialization.
    /// </summary>
    public interface IPayloadSerializerConsumer
    {
        IPayloadSerializationContainer PayloadSerializer { get; set; }
    }
}
