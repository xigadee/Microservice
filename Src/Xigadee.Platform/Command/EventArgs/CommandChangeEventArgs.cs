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
    /// This class contains the change information for the command.
    /// </summary>
    public class CommandChangeEventArgs: CommandEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandChangeEventArgs"/> class.
        /// </summary>
        /// <param name="isRemoval">if set to <c>true</c> [is removal].</param>
        /// <param name="key">The key.</param>
        /// <param name="isMasterJob">if set to <c>true</c> [is master job].</param>
        public CommandChangeEventArgs(bool isRemoval, MessageFilterWrapper key, bool isMasterJob)
        {
            IsRemoval = isRemoval;
            Key = key;
            IsMasterJob  = isMasterJob;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a removal.
        /// </summary>
        public bool IsRemoval { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is master job.
        /// </summary>
        public bool IsMasterJob { get; }
        /// <summary>
        /// Gets the key.
        /// </summary>
        public MessageFilterWrapper Key { get; }
    }
}
