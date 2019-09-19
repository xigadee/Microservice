using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Xigadee
{
    /// <summary>
    /// This class contains a number of Linq shortcuts.
    /// </summary>
    public static partial class LinqHelper
    {
        #region ForEach<T>(this IEnumerable<T> items, Action<T> action)
        /// <summary>
        /// The ForEach extension iterates through the items collection, and executes the action for each item.
        /// </summary>
        /// <example>
        /// A quick use of the method would be as follows:
        /// 
        ///     Enumerable.Range(0,40).ForEach(i => Console.WriteLine(i));
        ///     
        /// which is equivalent to the following code:
        /// 
        ///     foreach(var i in Enumerable.Range(0,40))
        ///         Console.WriteLine(i);
        /// </example>
        /// <typeparam name="T">The item type to process.</typeparam>
        /// <param name="items">The collection of items to process.</param>
        /// <param name="action">The action to be executed against each item in the collection.</param>
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) throw new ArgumentNullException("items", "The items enumeration cannot be null.");
            if (action == null) throw new ArgumentNullException("action", "The action delegate cannot be null.");

            foreach (var item in items)
                action(item);
        }
        #endregion

        #region ForIndex<T>(this IEnumerable<T> items, Action<int, T> action)
        /// <summary>
        /// The ForIndex extension method iterates through the items collection, and executes the action for each item and provides 
        /// a 32-bit integer index parameter that identifies the position of the item in the collection.
        /// </summary>
        /// <typeparam name="T">The item type to process.</typeparam>
        /// <param name="items">The collection of items to process.</param>
        /// <param name="action">The action to be executed against each item in the collection.</param>
        [DebuggerStepThrough]
        public static void ForIndex<T>(this IEnumerable<T> items, Action<int, T> action)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");
            if (action == null) throw new ArgumentNullException("action", "the action delegate is null");

            int index = 0;
            foreach (var item in items)
            {
                action(index, item);
                index++;
            }
        }

        /// <summary>
        /// Selects the item and returns its index position.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The incoming items.</param>
        /// <returns>Returns an enumeration with a ValueTuple.</returns>
        /// <exception cref="ArgumentNullException">items - items enumeration is null</exception>
        [DebuggerStepThrough]
        public static IEnumerable<(int index,T item)> SelectIndex<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");

            int index = 0;
            foreach (var item in items)
            {
                yield return (index, item);
                index++;
            }
        }
        #endregion
        #region ForBigIndex<T>(this IEnumerable<T> items, Action<long, T> action)
        /// <summary>
        /// The ForBigIndex extension method iterates through the items collection, and executes the action for each item and provides 
        /// a 64-bit integer parameter that identifies the position of the item in the collection.
        /// </summary>
        /// <typeparam name="T">The item type to process.</typeparam>
        /// <param name="items">The collection of items to process.</param>
        /// <param name="action">The action to be executed against each item in the collection.</param>
        [DebuggerStepThrough]
        public static void ForBigIndex<T>(this IEnumerable<T> items, Action<long, T> action)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");
            if (action == null) throw new ArgumentNullException("action", "the action delegate is null");

            long index = 0;
            foreach (var item in items)
            {
                action(index, item);
                index++;
            }
        }

        /// <summary>
        /// Selects the item and returns its index position.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The incoming items.</param>
        /// <returns>Returns an enumeration with a ValueTuple.</returns>
        /// <exception cref="ArgumentNullException">items - items enumeration is null</exception>
        [DebuggerStepThrough]
        public static IEnumerable<(long index, T item)> SelectBigIndex<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");

            long index = 0;
            foreach (var item in items)
            {
                yield return (index, item);
                index++;
            }
        }
        #endregion

        #region ForReverseIndex<T>(this IList<T> items, Action<int, T> action)
        /// <summary>
        /// The ForIndex extension method iterates through a list in reverse, and executes the action for each item and provides 
        /// a 32-bit integer index parameter that identifies the position of the item in the list.
        /// </summary>
        /// <typeparam name="T">The item type to process.</typeparam>
        /// <param name="items">The list of items to process.</param>
        /// <param name="action">The action to be executed against each item in the collection.</param>
        [DebuggerStepThrough]
        public static void ForReverseIndex<T>(this IList<T> items, Action<int, T> action)
        {

            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");
            if (action == null) throw new ArgumentNullException("action", "the action delegate is null");

            int length = items.Count;
            for (int index = length - 1; index >= 0; index--)
            {
                action(index, items[index]);
            }
        }
        #endregion

        /// <summary>
        /// This helper method allows for the quick comparison of a value against a set of generic values.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="compareset">The set of values to check for inclusion.</param>
        /// <returns>Returns true if set.</returns>
        public static bool IsIn<T>(this T value, params T[] compareset) => value.IsIn((r, t) => Equals(r, t), compareset);

        /// <summary>
        /// This helper method allows for the quick comparison of a value against a set of generic values.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="comparand">The function to evaluate a match.</param>
        /// <param name="compareset">The set of values to check for inclusion.</param>
        /// <returns>Returns true if in the set.</returns>
        public static bool IsIn<T>(this T value, Func<T, T, bool> comparand, params T[] compareset)
        {
            if (compareset == null || compareset.Length == 0)
                return false;

            if (comparand == null)
                throw new ArgumentNullException($"comparand cannot be null.");

            for (int i = 0; i < compareset.Length; i++)
            {
                if (comparand(compareset[i], value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// This helper method allows for the quick comparison of a value against a set of generic values.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="compareset">The set of values to check for exclusion.</param>
        /// <returns>Returns true if not in the set.</returns>
        public static bool IsNotIn<T>(this T value, params T[] compareset) => value.IsNotIn((r, t) => Equals(r, t), compareset);

        /// <summary>
        /// This helper method allows for the quick comparison of a value against a set of generic values.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="comparand">The function to evaluate a match.</param>
        /// <param name="compareset">The set of values to check for exclusion.</param>
        /// <returns>Returns true if not in the set.</returns>
        public static bool IsNotIn<T>(this T value, Func<T, T, bool> comparand, params T[] compareset)
        {
            if (compareset == null || compareset.Length == 0)
                return true;

            if (comparand == null)
                throw new ArgumentNullException($"comparand cannot be null.");

            for (int i = 0; i < compareset.Length; i++)
            {
                if (comparand(compareset[i], value))
                    return false;
            }

            return true;
        }
    }
}
