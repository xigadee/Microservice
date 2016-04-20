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
    public static class ConversionHelper
    {
        /// <summary>
        /// This static method is used for converting a date time string in to an object.
        /// </summary>
        /// <param name="data">The string.</param>
        /// <param name="styles">The style that the date is in default is to asssume that we are dealing with universal time UTC</param>
        /// <returns>The datetime object or null if the string cannot be converted.</returns>
        public static DateTime? ToDateTime(string data, DateTimeStyles styles= DateTimeStyles.AssumeUniversal)
        {
            DateTime val;

            if (DateTime.TryParse(data, CultureInfo.InvariantCulture, styles, out val ))
                return val;

            return null;
        }

        public static DateTime? ToDateTime(XElement el, DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            return el == null ? null : ToDateTime(el.Value, styles);
        }

        public static DateTime? ToDateTime(XAttribute el, DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            return el == null ? null : ToDateTime(el.Value, styles);
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

        public static Guid? ToGuid(XAttribute el)
        {
            return el == null ? null : ToGuid(el.Value);
        }

        public static Guid? ToGuid(XElement el)
        {
            return el == null ? null : ToGuid(el.Value);
        }

        public static string NodeNullable(XElement el, string value = null)
        {
            return el == null ? value : el.Value;
        }

        public static string NodeNullable(XAttribute el, string value = null)
        {
            return el == null ? value : el.Value;
        }

        public static void AddNonEmptyElement(XElement node, string name, string value)
        {
            if (node != null && !string.IsNullOrEmpty(value))
                node.Add(new XElement(name, value));
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

        public static long? ToLong(XElement el)
        {
            return el == null ? null : ToLong(el.Value);
        }

        public static long? ToLong(XAttribute el)
        {
            return el == null ? null : ToLong(el.Value);
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

        public static decimal? ToDecimal(XElement el)
        {
            return el == null ? null : ToDecimal(el.Value);
        }

        public static decimal? ToDecimal(XAttribute el)
        {
            return el == null ? null : ToDecimal(el.Value);
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

        public static int? ToInt(XElement el)
        {
            return el == null ? null : ToInt(el.Value);
        }

        public static int? ToInt(XAttribute el)
        {
            return el == null ? null : ToInt(el.Value);
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

        public static bool ToTrueBoolean(XElement el)
        {
            return el != null && ToTrueBoolean(el.Value);
        }

        public static bool ToTrueBoolean(XAttribute el)
        {
            return el != null && ToTrueBoolean(el.Value);
        }

        public static bool? ToBoolean(string data)
        {
            if (data == null)
                return null;

            return ToTrueBoolean(data);
        }

        public static bool? ToBoolean(XElement el)
        {
            return el == null ? null : ToBoolean(el.Value);
        }

        public static bool? ToBoolean(XAttribute el)
        {
            return el == null ? null : ToBoolean(el.Value);
        }

        public static string IsNullable(this string source, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(source))
                return defaultValue;
            else
                return source;
        }

        #region CalculateDelta(int now, int start)
        /// <summary>
        /// This method calculates the delta and takes in to account that the
        /// tickcount recycles to negative every 49 days.
        /// </summary>
        /// <param name="now"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int CalculateDelta(int now, int start)
        {
            int delta;
            if (now >= start)
                delta = now - start;
            else
            {
                //Do this, otherwise you'll be in a world of pain every 49 days.
                long upLimit = ((long)(int.MaxValue)) + Math.Abs(int.MinValue - now);
                delta = (int)(upLimit - start);
            }

            return delta;
        }
        #endregion

        public static int DeltaAsMs(int tickStart, int? tickNow = null)
        {
            return CalculateDelta(tickNow ?? Environment.TickCount, tickStart);
        }

        public static TimeSpan DeltaAsTimeSpan(int tickCount, int? tickStart = null)
        {
            return TimeSpan.FromMilliseconds(CalculateDelta(tickStart ?? Environment.TickCount, tickCount));
        }

        public static string DeltaAsFriendlyTime(int tickCount, int? tickStart = null)
        {
            return ToFriendlyString(DeltaAsTimeSpan(tickCount, tickStart));
        }

        #region FriendlyTime(TimeSpan? time, string defaultText="NA")

        static readonly Func<TimeSpan?, string> fnTimeConv = (time) =>
        {
            try
            {
                if (Math.Abs(time.Value.TotalMilliseconds) < 1000)
                    return string.Format("{0:F2}ms", time.Value.TotalMilliseconds);

                if (Math.Abs(time.Value.Days) > 0)
                    return time.Value.ToString(@"d'day'hh'h'mm'm'ss'.'ff's'");
                if (Math.Abs(time.Value.Hours) > 0)
                    return time.Value.ToString(@"hh'h'mm'm'ss'.'ff's'");
                if (Math.Abs(time.Value.Minutes) > 0)
                    return time.Value.ToString(@"mm'm'ss'.'ff's'");

                return time.Value.ToString(@"ss'.'ff's'");
            }
            catch (Exception)
            {
                return null;
            }
        };
        /// <summary>
        /// This helper converts a timespan in to a human readable time.
        /// </summary>
        /// <param name="time">The TimeSpan object to convert.</param>
        /// <param name="defaultText">The default text to display if the TimeSpan object is null. NA by default.</param>
        /// <returns>Returns a string representation of the time.</returns>
        public static string ToFriendlyString(this TimeSpan? timeIn, string nullName = "NA")
        {
            return timeIn.HasValue? ToFriendlyString(timeIn.Value): nullName;
        }
        /// <summary>
        /// This helper converts a timespan in to a human readable time.
        /// </summary>
        /// <param name="time">The TimeSpan object to convert.</param>
        /// <returns>Returns a string representation of the time.</returns>
        public static string ToFriendlyString(this TimeSpan timeIn)
        {
            string output = fnTimeConv(timeIn);

            if (output == null)
                return "ERR";

            if (timeIn.TotalMilliseconds < 0)
                return "-" + output;

            return output;
        }
        #endregion

    }
}
