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
using System.Configuration;
#endregion
namespace Xigadee
{
    /// <summary>
    /// THis exception is thrown when a key is not found when throwExceptionIfNotFound is set to true.
    /// </summary>
    public class ConfigResolverKeyNotFoundException:Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="key">The key that could not be resolved.</param>
        public ConfigResolverKeyNotFoundException(string key):base($"The configuration key '{key}' cannot be resolved.")
        {
            Key = key;
        }

        /// <summary>
        /// The missing key.
        /// </summary>
        public string Key { get; }
    }
}
