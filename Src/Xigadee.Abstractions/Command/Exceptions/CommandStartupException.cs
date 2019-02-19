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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class CommandStartupException: CommandException
    {
        /// <summary>
        /// Initializes a new instance of the CommandStartupException class.
        /// </summary>
        public CommandStartupException() : base() { }
        /// <summary>
        /// Initializes a new instance of the CommandStartupException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CommandStartupException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the CommandStartupException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public CommandStartupException(string message, Exception ex) : base(message, ex) { }
    }
}
