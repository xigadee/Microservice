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
    /// This class is used to log and event source entry.
    /// </summary>
    public class EventSourceEntry: EventSourceEntry<object, object>
    {

    }

    public class EventSourceEntry<K, E>: EventSourceEntryBase
    {
        private Func<K, string> mKeyMaker;

        public EventSourceEntry()
        {
            mKeyMaker = null;
        }
        public EventSourceEntry(Func<K, string> keyMaker = null)
        {
            mKeyMaker = keyMaker;
        }

        public K EntityKey { get; set; }

        public E Entity { get; set; }

        public override string Key
        {
            get
            {
                return mKeyMaker == null ? EntityKey.ToString() : mKeyMaker(EntityKey);
            }
        }
    }
}
