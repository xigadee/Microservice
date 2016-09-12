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
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to hold a key/value reference.
    /// </summary>
    public class EntityCacheReferenceTypeValueKey: IEquatable<EntityCacheReferenceTypeValueKey>
    {
        /// <summary>
        /// This is the reference type, i.e. EMAIL, CUSTOMERID etc.
        /// </summary>
        public string RefType
        {
            get; set;
        }
        /// <summary>
        /// This is the reference value.
        /// </summary>
        public string RefValue
        {
            get; set;
        }
        /// <summary>
        /// This specific override is used to compare two instances for equality.
        /// </summary>
        /// <param name="other">The other key instance.</param>
        /// <returns>Returns true if they match.</returns>
        public bool Equals(EntityCacheReferenceTypeValueKey other)
        {
            return other != null
                && string.Equals(RefType, other.RefType, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(RefValue, other.RefValue, StringComparison.InvariantCulture);
        }
    }
}
