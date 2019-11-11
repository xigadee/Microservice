using System;
namespace Xigadee
{
    /// <summary>
    /// This class contains a number of Linq shortcuts.
    /// </summary>
    public static partial class SetHelper
    {
        #region IsIn
        /// <summary>
        /// This helper method allows for the quick comparison of a value against a set of generic values.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="compareset">The set of values to check for inclusion.</param>
        /// <returns>Returns true if set.</returns>
        public static bool IsIn(this string value, params string[] compareset) => IsIn(value, (r, t) => Equals(r, t), compareset);

        public static bool IsIn(this int value, params int[] compareset) => IsIn(value, (r, t) => Equals(r, t), compareset);

        public static bool IsNotIn(this int value, params int[] compareset) => !value.IsIn(compareset);

        public static bool IsIn(this long value, params long[] compareset) => IsIn(value, (r, t) => Equals(r, t), compareset);

        public static bool IsNotIn(this long value, params long[] compareset) => !value.IsIn(compareset);

        /// <summary>
        /// Enum specific matching.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">THe value to match.</param>
        /// <param name="compareset">The comparison set.</param>
        /// <returns>Returns true if there is a match.</returns>
        public static bool IsIn<T>(this T value, params T[] compareset) where T: System.Enum => IsIn(value, (r, t) => Enum.Equals(r, t), compareset);

        /// <summary>
        /// This helper method allows for the quick comparison of a value against a set of generic values.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="comparand">The function to evaluate a match.</param>
        /// <param name="compareset">The set of values to check for inclusion.</param>
        /// <returns>Returns true if in the set.</returns>
        public static bool IsIn<T>(T value, Func<T, T, bool> comparand, params T[] compareset)
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
        #endregion
        #region IsNotIn
        /// <summary>
        /// This helper method allows for the quick comparison of a value against a set of generic values.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="compareset">The set of values to check for exclusion.</param>
        /// <returns>Returns true if not in the set.</returns>
        public static bool IsNotIn(this string value, params string[] compareset) => IsNotIn(value,(r, t) => Equals(r, t), compareset);
        /// <summary>
        /// Enum specific matching.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">THe value to match.</param>
        /// <param name="compareset">The comparison set.</param>
        /// <returns>Returns true if there is not a match.</returns>
        public static bool IsNotIn<T>(this T value, params T[] compareset) where T : System.Enum => IsNotIn(value, (r, t) => Equals(r, t), compareset);
        /// <summary>
        /// This helper method allows for the quick comparison of a value against a set of generic values.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="comparand">The function to evaluate a match.</param>
        /// <param name="compareset">The set of values to check for exclusion.</param>
        /// <returns>Returns true if not in the set.</returns>
        public static bool IsNotIn<T>(T value, Func<T, T, bool> comparand, params T[] compareset)
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
        #endregion

        /// <summary>
        /// This is a special comparison for flag based enumerations.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">The value to compare.</param>
        /// <param name="compareset">The set of values to compare.</param>
        /// <returns>Returns true on a match.</returns>
        public static bool IsInFlag<T>(this T value, params T[] compareset) where T : System.Enum => IsIn(value, (r, t) => EnumMatch(r, t), compareset);
        /// <summary>
        /// This is a special comparison for flag based enumerations.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">The value to compare.</param>
        /// <param name="compareset">The set of values to compare.</param>
        /// <returns>Returns true if there is not a match.</returns>
        public static bool IsNotInFlag<T>(this T value, params T[] compareset) where T : System.Enum => IsNotIn(value, (r, t) => EnumMatch(r, t), compareset);

        private static bool EnumMatch<T>(T op2, T op1) where T : System.Enum
        {
            var ut = Enum.GetUnderlyingType(typeof(T));
            
            //Let's play nice.
            if (IsIn(ut, (a,b) => a == b, typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong)))
                //return (Convert.ToUInt64(op1) & Convert.ToUInt64(op2)) > 0;
            return (Convert.ToUInt64(op1) & Convert.ToUInt64(op2)) == Convert.ToUInt64(op1);

            //return (Convert.ToInt64(op1) & Convert.ToInt64(op2)) > 0;
            return (Convert.ToInt64(op1) & Convert.ToInt64(op2)) == Convert.ToInt64(op1);
        }
    }
}
