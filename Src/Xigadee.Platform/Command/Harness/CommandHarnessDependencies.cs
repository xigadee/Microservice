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
    /// This class sets the dependencies for the command,
    /// </summary>
    /// <typeparam name="C">The command type.</typeparam>
    /// <seealso cref="Xigadee.ServiceHarnessDependencies" />
    public class CommandHarnessDependencies<C>: ServiceHarnessDependencies
    {
        /// <summary>
        /// Gets the creator function for the harness service.
        /// </summary>
        public Func<C> Creator { get; }

        public CommandHarnessDependencies() : this(null)
        {
        }

        public CommandHarnessDependencies(Func<C> creator)
        {
            Creator = creator ?? DefaultConstructor();
        }

        /// <summary>
        /// This method checks whether the command supports a parameterless constructor.
        /// </summary>
        /// <returns>Returns the command.</returns>
        private Func<C> DefaultConstructor()
        {
            if (typeof(C).GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentOutOfRangeException($"The command {typeof(C).Name} does not support a parameterless constructor. Please supply a creator function.");

            return () => Activator.CreateInstance<C>();
        }

        /// <summary>
        /// This is the example originator id.
        /// </summary>
        public override MicroserviceId OriginatorId => new MicroserviceId("CommandHarness", Guid.NewGuid().ToString("N").ToUpperInvariant());
    }
}
