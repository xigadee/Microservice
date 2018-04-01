using System;
using System.Collections.Generic;

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
