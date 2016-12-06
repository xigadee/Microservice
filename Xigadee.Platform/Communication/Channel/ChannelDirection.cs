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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This enumeration specifies the direction of the message.
    /// </summary>
    [Flags]
    public enum ChannelDirection:int
    {
        /// <summary>
        /// The message has been received from an external source.
        /// </summary>
        Incoming = 1,
        /// <summary>
        /// The message has just been transmitted to an external source.
        /// </summary>
        Outgoing = 2,
        /// <summary>
        /// This channel supports incoming and outgoing flow.
        /// </summary>
        Bidirectional = 3
    }
}
