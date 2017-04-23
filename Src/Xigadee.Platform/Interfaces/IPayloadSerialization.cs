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
    /// <summary>
    /// This is the interface used to serialize and deserialize payloads.
    /// </summary>
    public interface IPayloadSerializer: IDisposable
    {
        byte[] Identifier { get; }

        IEnumerable<byte[]> PayloadMagicNumbers();

        bool SupportsObjectTypeSerialization(Type entityType);

        bool SupportsPayloadDeserialization(byte[] blob);

        bool SupportsPayloadDeserialization(byte[] blob, int start, int length);

        E Deserialize<E>(byte[] blob);

        E Deserialize<E>(byte[] blob, int start, int length);

        object Deserialize(byte[] blob);

        object Deserialize(byte[] blob, int start, int length);

        byte[] Serialize<E>(E entity);

        byte[] Serialize(object entity);

    }
}
