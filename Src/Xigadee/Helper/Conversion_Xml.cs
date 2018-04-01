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
    /// This static class is used to perform specific conversion steps, specifically from XML based data.
    /// </summary>
    public static partial class ConversionHelper
    {

        public static DateTime? ToDateTime(XElement el, DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            return el == null ? null : ToDateTime(el.Value, styles);
        }

        public static DateTime? ToDateTime(XAttribute el, DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            return el == null ? null : ToDateTime(el.Value, styles);
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

        public static long? ToLong(XElement el)
        {
            return el == null ? null : ToLong(el.Value);
        }

        public static long? ToLong(XAttribute el)
        {
            return el == null ? null : ToLong(el.Value);
        }


        public static decimal? ToDecimal(XElement el)
        {
            return el == null ? null : ToDecimal(el.Value);
        }

        public static decimal? ToDecimal(XAttribute el)
        {
            return el == null ? null : ToDecimal(el.Value);
        }


        public static int? ToInt(XElement el)
        {
            return el == null ? null : ToInt(el.Value);
        }

        public static int? ToInt(XAttribute el)
        {
            return el == null ? null : ToInt(el.Value);
        }

        public static bool ToTrueBoolean(XElement el)
        {
            return el != null && ToTrueBoolean(el.Value);
        }

        public static bool ToTrueBoolean(XAttribute el)
        {
            return el != null && ToTrueBoolean(el.Value);
        }

        public static bool? ToBoolean(XElement el)
        {
            return el == null ? null : ToBoolean(el.Value);
        }

        public static bool? ToBoolean(XAttribute el)
        {
            return el == null ? null : ToBoolean(el.Value);
        }

        public static double? ToDouble(XElement el)
        {
            return el == null ? null : ToDouble(el.Value);
        }

        public static double? ToDouble(XAttribute el)
        {
            return el == null ? null : ToDouble(el.Value);
        }
    }
}
