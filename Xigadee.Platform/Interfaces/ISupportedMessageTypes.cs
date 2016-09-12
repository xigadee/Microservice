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
    public interface ISupportedMessageTypes
    {
        /// <summary>
        /// This event is used to signal a change of registered commands.
        /// </summary>
        event EventHandler<SupportedMessagesChange> OnCommandChange;
        /// <summary>
        /// This is the supported message types for the listener.
        /// </summary>
        List<MessageFilterWrapper> SupportedMessages { get; }
    }

    public class SupportedMessagesChange
    {
        public List<MessageFilterWrapper> Messages { get; set; }
    }
}
