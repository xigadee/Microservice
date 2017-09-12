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
    public class CommandHarnessDependencies: ServiceHarnessDependencies
    {

        /// <summary>
        /// This is the example originator id.
        /// </summary>
        public override MicroserviceId OriginatorId => new MicroserviceId("CommandHarness", Guid.NewGuid().ToString("N").ToUpperInvariant());
    }
}
