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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This context holds the logging information for the console.
    /// </summary>
    public class ConsoleInfoContext
    {
        /// <summary>
        /// This is the list of info messages.
        /// </summary>
        public ConcurrentBag<ErrorInfo> InfoMessages { get; } = new ConcurrentBag<ErrorInfo>();

        public bool InfoDecrement()
        {
            if (InfoCurrent == 0)
                return false;

            InfoCurrent--;

            return true;
        }

        public bool InfoIncrement()
        {
            if (InfoCurrent == InfoMax - 1)
                return false;

            InfoCurrent++;

            return true;
        }

        public int InfoCurrent { get; set; }

        public int InfoMax
        {
            get
            {
                return InfoMessages.Count;
            }
        }

        public bool Refresh { get; set; }
    }
}
