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
    /// This exception is used to signal when the ChannelId has not been set, but you are using partial attributes.
    /// </summary>
    /// <seealso cref="Xigadee.CommandException" />
    public class CommandChannelIdNullException: CommandException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandChannelIdNullException"/> class.
        /// </summary>
        public CommandChannelIdNullException(string message):base(message)
        {

        }
    }
}
