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
        #region FindFirstPosition<TSource>(this IList<TSource> source, Func<TSource, bool> predicate)
        /// <summary>
        /// This extension method will find the first position in the list based on the predicate.
        /// </summary>
        /// <typeparam name="TSource">The object type.</typeparam>
        /// <param name="source">The array list.</param>
        /// <param name="predicate">The match condition.</param>
        /// <returns>Return the position in the list, or -1 if the predicate cannot be matched.</returns>
        public static int FindFirstPosition<TSource>(this IList<TSource> source, Func<TSource, bool> predicate)
        {
            return FindPositionInternal<TSource>(source, 0, source.Count, predicate);
        }
        #endregion  
        #region FindFirstPosition<TSource>(this IList<TSource> source, int offset, int count, Func<TSource, bool> predicate)
        /// <summary>
        /// This extension method will find the first position in the list based on the predicate.
        /// </summary>
        /// <typeparam name="TSource">The object type.</typeparam>
        /// <param name="source">The array list.</param>
        /// <param name="offset">The list offset.</param>
        /// <param name="count">The number of items to process.</param>
        /// <param name="predicate">The match condition.</param>
        /// <returns>Return the position in the list, or -1 if the predicate cannot be matched.</returns>
        public static int FindFirstPosition<TSource>(this IList<TSource> source, int offset, int count, Func<TSource, bool> predicate)
        {
            return FindPositionInternal<TSource>(source, offset, count, predicate);
        }
        #endregion  

        #region FindPositionInternal<TSource>(IList<TSource> source, int offset, int count, Func<TSource, bool> predicate)
        /// <summary>
        /// This extension method will find the first position in the list based on the predicate and the 
        /// boundary fields passed.
        /// </summary>
        /// <typeparam name="TSource">The source item type.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="offset">The list search start offset.</param>
        /// <param name="count">The number of items to check.</param>
        /// <param name="predicate">The predicate function to check for equality.</param>
        /// <returns>Returns the position of the item that matched.</returns>
        static int FindPositionInternal<TSource>(IList<TSource> source, int offset, int count, Func<TSource, bool> predicate)
        {
            int num = offset + count;
            int i;

            for (i = offset; i < num; i++)
            {
                if (predicate(source[i]))
                    break;
            }

            return i == num ? -1 : i;
        }
        #endregion  
    }
}
