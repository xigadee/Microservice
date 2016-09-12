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
    /// This class holders a registered process that will be polled as part of the thread loop.
    /// </summary>
    public class TaskManagerProcessContext
    {
        public TaskManagerProcessContext(string name)
        {
            Name = name;
        }
        /// <summary>
        /// The process priority.
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// The execute action.
        /// </summary>
        public ITaskManagerProcess Process { get; set; }
        /// <summary>
        /// The unique readonly process name.
        /// </summary>
        public string Name { get; }
    }

    public class TaskManagerProcessContext<C>: TaskManagerProcessContext
    {
        public TaskManagerProcessContext(string name) : base(name)
        {
            Context = default(C);
        }

        public C Context { get; set; }
    }
}
