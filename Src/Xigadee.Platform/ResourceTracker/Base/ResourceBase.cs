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
    /// This class is used by resource objects.
    /// </summary>
    public abstract class ResourceBase: IResourceBase
    {
        /// <summary>
        /// The unique id of the resource.
        /// </summary>
        public Guid ResourceId { get; } = Guid.NewGuid();
        /// <summary>
        /// The name of the resource.
        /// </summary>
        public string Name { get; protected set;}
    }
}
