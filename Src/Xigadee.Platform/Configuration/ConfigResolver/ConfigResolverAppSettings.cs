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
    /// This resolver pulls takes configuration from the configuration manager.
    /// </summary>
    [Obsolete("Removing for .NET Standard compatibility")]
    public class ConfigResolverAppSettings: ConfigResolver
    {
        /// <summary>
        /// This method returns true if the key is set.
        /// </summary>
        /// <param name="key">The key setting.</param>
        /// <returns>True if set.</returns>
        [Obsolete("Removing for .NET Standard compatibility")]
        public override bool CanResolve(string key)
        {
            return ConfigurationManager.AppSettings[key] != null;
        }

        /// <summary>
        /// This method resolves the key value.
        /// </summary>
        /// <param name="key">The key setting.</param>
        /// <returns>The value of the key.</returns>
        [Obsolete("Removing for .NET Standard compatibility")]
        public override string Resolve(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
