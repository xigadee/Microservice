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
namespace Xigadee
{
    /// <summary>
    /// This is the abstract base config resolver class. Override this class to sort this out.
    /// </summary>
    public abstract class ConfigResolver
    {
        /// <summary>
        /// Use this method to get the value from the specific resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>Returns true if it can resolve.</returns>
        public abstract bool CanResolve(string key);
        /// <summary>
        /// Use this method to get the value from the specific resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>This is the settings value, null if not set.</returns>
        public abstract string Resolve(string key);
    }
}
