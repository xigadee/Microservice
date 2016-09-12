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
    /// This is the abstract class used by the primary Microservice containers.
    /// </summary>
    /// <typeparam name="S">The status class.</typeparam>
    /// <typeparam name="P">The policy class.</typeparam>
    public abstract class ServiceContainerBase<S,P>:ServiceBase<S>
        where S : StatusBase, new()
        where P : PolicyBase, new()
    {
        /// <summary>
        /// This is the container policy.
        /// </summary>
        protected P mPolicy;

        /// <summary>
        /// This is the default construct that sets or creates the policy object depending on whether it is passed in to the constructor. 
        /// </summary>
        /// <param name="policy">The optional policy class.</param>
        /// <param name="name">The optional name for the component.</param>
        public ServiceContainerBase(P policy = null, string name = null):base(name)
        {
            mPolicy = policy ?? new P();
        }
    }
}
