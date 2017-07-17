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

using System.Text;
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to provide a consistent interface for initializing an entity from a byte stream or array.
    /// </summary>
    public interface IMessageLoadData
    {
        int Load(string data);

        int Load(string data, Encoding encoding);

        int Load(byte[] buffer, int offset, int count);

        int Load(Stream data);
   }

    public interface IMessageLoadData<TERM>
        where TERM : IMessageTermination
    {
        int Load(TERM terminator, byte[] buffer, int offset, int count);

        int Load(TERM terminator, Stream data);

        int Load(TERM terminator, string data);

        int Load(TERM terminator, string data, Encoding encoding);
    }
    
}
