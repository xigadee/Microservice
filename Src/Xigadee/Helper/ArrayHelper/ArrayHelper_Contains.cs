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
    public static partial class ArrayHelper
    {
        #region Contains<T>(this IEnumerable<T> items, Predicate<T> action)
        /// <summary>
        /// This method scans a collection and returns true when an item is matched.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="items">The enumeration.</param>
        /// <param name="predic">The predicate that returns true when there is a match.</param>
        /// <returns>Returns true if an item is matched in the collection.</returns>
        public static bool Contains<T>(this IEnumerable<T> items, Predicate<T> predic)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predic == null) throw new ArgumentNullException("action");

            foreach (T item in items)
                if (predic(item))
                    return true;

            return false;
        }
        #endregion  
    }
}
