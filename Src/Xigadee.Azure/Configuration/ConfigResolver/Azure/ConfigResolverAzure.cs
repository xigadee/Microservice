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

using Microsoft.Azure;

namespace Xigadee
{
    /// <summary>
    /// Simple resolver that reads settings using the CloudConfigurationManager.
    /// </summary>
    public class ConfigResolverAzure: ConfigResolver
    {
        /// <summary>
        /// Use this method to identify whether the key exists.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>
        /// Returns true if it can resolve.
        /// </returns>
        public override bool CanResolve(string key)
        {
            return CloudConfigurationManager.GetSetting(key) != null;
        }
        /// <summary>
        /// Use this method to get the value.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>
        /// This is the settings value, null if not set.
        /// </returns>
        public override string Resolve(string key)
        {
            return CloudConfigurationManager.GetSetting(key);
        }
    }
}
