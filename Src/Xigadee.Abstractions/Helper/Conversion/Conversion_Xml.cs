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
    public static partial class ConversionHelper
    {
        /// <summary>
        /// Converts to date time.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="styles">The datetime styles.</param>
        /// <returns>Returns a nullable Datetime.</returns>
        public static DateTime? ToDateTime(XElement el, DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            return el == null ? null : ToDateTime(el.Value, styles);
        }
        /// <summary>
        /// Converts to date time.
        /// </summary>
        /// <param name="el">The attribute.</param>
        /// <param name="styles">The datetime styles.</param>
        /// <returns>Returns a nullable datetime.</returns>
        public static DateTime? ToDateTime(XAttribute el, DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            return el == null ? null : ToDateTime(el.Value, styles);
        }

        /// <summary>
        /// Converts to a unique identifier.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable Guid.</returns>
        public static Guid? ToGuid(XAttribute el)
        {
            return el == null ? null : ToGuid(el.Value);
        }
        /// <summary>
        /// Converts to a unique identifier.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable Guid.</returns>
        public static Guid? ToGuid(XElement el)
        {
            return el == null ? null : ToGuid(el.Value);
        }
        /// <summary>
        /// Returns a string from a nullable Xml node.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="value">The default value.</param>
        /// <returns>Returns a string value or the default value.</returns>
        public static string NodeNullable(XElement el, string value = null)
        {
            return el == null ? value : el.Value;
        }
        /// <summary>
        /// Returns a string from a nullable Xml node.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="value">The default value.</param>
        /// <returns>Returns a string value or the default value.</returns>
        public static string NodeNullable(XAttribute el, string value = null)
        {
            return el == null ? value : el.Value;
        }
        /// <summary>
        /// Adds the non empty element.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void AddNonEmptyElement(XElement node, string name, string value)
        {
            if (node != null && !string.IsNullOrEmpty(value))
                node.Add(new XElement(name, value));
        }
        /// <summary>
        /// Converts to a long.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable long.</returns>
        public static long? ToLong(XElement el)
        {
            return el == null ? null : ToLong(el.Value);
        }
        /// <summary>
        /// Converts to a long.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable long.</returns>
        public static long? ToLong(XAttribute el)
        {
            return el == null ? null : ToLong(el.Value);
        }

        /// <summary>
        /// Converts to a decimal.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable decimal.</returns>
        public static decimal? ToDecimal(XElement el)
        {
            return el == null ? null : ToDecimal(el.Value);
        }

        /// <summary>
        /// Converts to a decimal.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable decimal.</returns>
        public static decimal? ToDecimal(XAttribute el)
        {
            return el == null ? null : ToDecimal(el.Value);
        }

        /// <summary>
        /// Converts to a int.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable int.</returns>
        public static int? ToInt(XElement el)
        {
            return el == null ? null : ToInt(el.Value);
        }
        /// <summary>
        /// Converts to a int.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable int.</returns>
        public static int? ToInt(XAttribute el)
        {
            return el == null ? null : ToInt(el.Value);
        }
        /// <summary>
        /// Converts to a boolean only if the value matches 'true'.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a boolean value.</returns>
        public static bool ToTrueBoolean(XElement el)
        {
            return el != null && ToTrueBoolean(el.Value);
        }
        /// <summary>
        /// Converts to a boolean only if the value matches 'true'.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a boolean value.</returns>
        public static bool ToTrueBoolean(XAttribute el)
        {
            return el != null && ToTrueBoolean(el.Value);
        }
        /// <summary>
        /// Converts to a bool.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable bool.</returns>
        public static bool? ToBoolean(XElement el)
        {
            return el == null ? null : ToBoolean(el.Value);
        }
        /// <summary>
        /// Converts to a bool.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable bool.</returns>
        public static bool? ToBoolean(XAttribute el)
        {
            return el == null ? null : ToBoolean(el.Value);
        }
        /// <summary>
        /// Converts to a double.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable double.</returns>
        public static double? ToDouble(XElement el)
        {
            return el == null ? null : ToDouble(el.Value);
        }
        /// <summary>
        /// Converts to a double.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Returns a nullable double.</returns>
        public static double? ToDouble(XAttribute el)
        {
            return el == null ? null : ToDouble(el.Value);
        }
    }
}
