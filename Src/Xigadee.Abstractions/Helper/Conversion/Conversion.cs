#region using
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
#endregion

namespace Xigadee
{
    /// <summary>
    /// This library is used to perform conversion methods.
    /// </summary>
    public static partial class ConversionHelper
    {
        /// <summary>
        /// This static method is used for converting a date time string in to an object.
        /// </summary>
        /// <param name="data">The string.</param>
        /// <param name="styles">The style that the date is in default is to assume that we are dealing with universal time UTC</param>
        /// <returns>The datetime object or null if the string cannot be converted.</returns>
        public static DateTime? ToDateTime(string data, DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            DateTime val;

            if (DateTime.TryParse(data, CultureInfo.InvariantCulture, styles, out val))
                return val;

            return null;
        }
        /// <summary>
        /// Converts to a long.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>Returns a nullable long.</returns>
        public static long? ToLong(string data)
        {
            if (data == null)
                return null;

            long val;

            if (long.TryParse(data, out val))
                return val;

            return null;
        }
        /// <summary>
        /// Converts to a Guid.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>Returns a nullable Guid.</returns>
        public static Guid? ToGuid(string data)
        {
            if (data == null)
                return null;

            Guid val;

            if (Guid.TryParse(data, out val))
                return val;

            return null;
        }

        /// <summary>
        /// Converts to a decimal.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>Returns a nullable decimal.</returns>
        public static decimal? ToDecimal(string data)
        {
            if (data == null)
                return null;

            decimal val;

            if (decimal.TryParse(data, out val))
                return val;

            return null;
        }
        /// <summary>
        /// Converts to a double.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>Returns a nullable double.</returns>
        public static double? ToDouble(string data)
        {
            if (data == null)
                return null;

            double val;

            if (double.TryParse(data, out val))
                return val;

            return null;
        }

        /// <summary>
        /// Converts to a int.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>Returns a nullable int.</returns>
        public static int? ToInt(string data)
        {
            if (data == null)
                return null;

            int val;

            if (int.TryParse(data, out val))
                return val;

            return null;
        }

        /// <summary>
        /// Converts to a boolean.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>Returns a nullable boolean.</returns>
        public static bool? ToBoolean(string data)
        {
            if (data == null)
                return null;

            return ToTrueBoolean(data);
        }
        /// <summary>
        /// Converts to a boolean only if the value can be matched to a known true value.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>Returns a boolean value, true on match, false otherwise.</returns>
        public static bool ToTrueBoolean(string data)
        {
            if (data == null)
                return false;

            int val;
            if (int.TryParse(data, out val))
                return Convert.ToBoolean(val);

            bool boolVal;
            return Boolean.TryParse(data, out boolVal) && boolVal;
        }
        /// <summary>
        /// Determines whether the source is nullable.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>If source is null, then the default value is returned.</returns>
        public static string IsNullable(this string source, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(source))
                return defaultValue;
            else
                return source;
        }
    }
}
