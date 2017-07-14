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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This exception is used during start up when validating the messaging setting
    /// </summary>
    public class StartupMessagingException:MessagingException
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="property">The property that has caused the error.</param>
        /// <param name="message">The error message.</param>
        public StartupMessagingException(string property, string message):base(message)
        {
            Property = property;
        }
        /// <summary>
        /// This is the property that has caused the error.
        /// </summary>
        public string Property { get; }
    }

    /// <summary>
    /// This exception is used during start up when validating the messaging setting
    /// </summary>
    public class ClientsUndefinedMessagingException: MessagingException
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ClientsUndefinedMessagingException(string message) : base(message)
        {
        }

    }
}
