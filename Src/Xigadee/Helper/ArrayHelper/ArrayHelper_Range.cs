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
    /// This static class provides a number of extension method for array objects.
    /// </summary>
    public static partial class ArrayHelper
    {
        #region Range<TSource>(this IList<TSource> source, int offset, int count)
        /// <summary>
        /// This extension selects a range of array values based on the offset and the count value.
        /// </summary>
        /// <typeparam name="TSource">This extension method can be applied to any object that implements the IList interface.</typeparam>
        /// <param name="source">The array source.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="count">The number of records to process.</param>
        /// <returns>Returns a enumerable collection containing the records.</returns>
        public static IEnumerable<TSource> Range<TSource>(this IList<TSource> source, int offset, int count)
        {
            int num = offset + count;
            for (int i = offset; i < num; i++)
            {
                yield return source[i];
            }
        }
        #endregion  
    }
}
