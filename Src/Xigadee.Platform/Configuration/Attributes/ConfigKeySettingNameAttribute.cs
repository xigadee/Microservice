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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to annotate a shortcut for a key for a particular group.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigSettingKeyAttribute:Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSettingKeyAttribute"/> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="description">The description.</param>
        public ConfigSettingKeyAttribute(string category = null, string description = null)
        {
            Category = category;
            Description = description;
        }
        /// <summary>
        /// Gets the category for the key used to group them together.
        /// </summary>
        public string Category { get; }
        /// <summary>
        /// Gets the description for the key.
        /// </summary>
        public string Description { get; }
    }

    /// <summary>
    /// This attribute is used to identify a specific method that is connected to a shortcut key.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class ConfigSettingAttribute: Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSettingAttribute"/> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="description">The description.</param>
        public ConfigSettingAttribute(string category, string description = null)
        {
            Category = category;
            Description = description;
        }
        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category { get; }
        /// <summary>
        /// Gets the description for the key.
        /// </summary>
        public string Description { get; }
    }
}
