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
    public  static partial class ConversionHelper
    {
        /// <summary>
        /// This static method is used for converting a date time string in to an object.
        /// </summary>
        /// <param name="data">The string.</param>
        /// <param name="styles">The style that the date is in default is to asssume that we are dealing with universal time UTC</param>
        /// <returns>The datetime object or null if the string cannot be converted.</returns>
        public static DateTime? ToDateTime(string data, DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            DateTime val;

            if (DateTime.TryParse(data, CultureInfo.InvariantCulture, styles, out val))
                return val;

            return null;
        }

        public static long? ToLong(string data)
        {
            if (data == null)
                return null;

            long val;

            if (long.TryParse(data, out val))
                return val;

            return null;
        }

        public static Guid? ToGuid(string data)
        {
            if (data == null)
                return null;

            Guid val;

            if (Guid.TryParse(data, out val))
                return val;

            return null;
        }


        public static decimal? ToDecimal(string data)
        {
            if (data == null)
                return null;

            decimal val;

            if (decimal.TryParse(data, out val))
                return val;

            return null;
        }


        public static int? ToInt(string data)
        {
            if (data == null)
                return null;

            int val;

            if (int.TryParse(data, out val))
                return val;

            return null;
        }


        public static bool? ToBoolean(string data)
        {
            if (data == null)
                return null;

            return ToTrueBoolean(data);
        }

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

        public static string IsNullable(this string source, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(source))
                return defaultValue;
            else
                return source;
        }
    }
}
