using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The <b>Common</b> class includes a number of useful utilities.
    /// </summary>
    public static class StringHelper
    {
        #region XmlDecode()
        /// <summary>
        /// Handle the special xml decode.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string XmlDecode(string source)
        {
            return string.IsNullOrEmpty(source) ? source : source.Replace("&lt;", "<").Replace("&gt;", ">").Replace(@"\n", "\n");
        }
        #endregion
#if (!SILVERLIGHT)
        #region StringToTypedValue
        /// <summary>
        /// Turns a string into a typed value. Useful for auto-conversion routines
        /// like form variable or XML parsers.
        /// <seealso>Class wwUtils</seealso>
        /// </summary>
        /// <param name="SourceString">
        /// The string to convert from
        /// </param>
        /// <param name="TargetType">
        /// The type to convert to
        /// </param>
        /// <param name="Culture">
        /// Culture used for numeric and datetime values.
        /// </param>
        /// <returns>object. Throws exception if it cannot be converted.</returns>
        public static object StringToTypedValue(string SourceString, Type TargetType, CultureInfo Culture)
        {
            object Result = null;

            if (TargetType == typeof(string))
                Result = SourceString;
            else if (TargetType == typeof(int))
                Result = int.Parse(SourceString, NumberStyles.Integer, Culture.NumberFormat);
            else if (TargetType == typeof(byte))
                Result = Convert.ToByte(SourceString);
            else if (TargetType == typeof(decimal))
                Result = Decimal.Parse(SourceString, NumberStyles.Any, Culture.NumberFormat);
            else if (TargetType == typeof(double))
                Result = Double.Parse(SourceString, NumberStyles.Any, Culture.NumberFormat);
            else if (TargetType == typeof(bool))
            {
                if (SourceString.ToLower() == "true" || SourceString.ToLower() == "on" || SourceString == "1")
                    Result = true;
                else
                    Result = false;
            }
            else if (TargetType == typeof(DateTime))
                Result = Convert.ToDateTime(SourceString, Culture.DateTimeFormat);
            else if (TargetType.IsEnum)
                Result = Enum.Parse(TargetType, SourceString, true);
            else
            {
                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(TargetType);
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                    Result = converter.ConvertFromString(null, Culture, SourceString);
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Type Conversion not handled in StringToTypedValue for " +
                                                    TargetType.Name + " " + SourceString);
                    throw (new Exception("Type Conversion not handled in StringToTypedValue"));
                }
            }

            return Result;
        }

        /// <summary>
        /// Turns a string into a typed value. Useful for auto-conversion routines
        /// like form variable or XML parsers.
        /// </summary>
        /// <param name="SourceString">The input string to convert</param>
        /// <param name="TargetType">The Type to convert it to</param>
        /// <returns>object reference. Throws Exception if type can not be converted</returns>
        public static object StringToTypedValue(string SourceString, Type TargetType)
        {
            return StringToTypedValue(SourceString, TargetType, CultureInfo.CurrentCulture);
        }
        #endregion
#endif


        #region SplitOnChars
        /// <summary>
        /// This method is used to split string pairs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="toSplit"></param>
        /// <param name="convertT"></param>
        /// <param name="convertU"></param>
        /// <param name="split1"></param>
        /// <param name="split2"></param>
        /// <returns></returns>
        public static List<KeyValuePair<T, U>> SplitOnChars<T, U>(string toSplit,
            Converter<string, T> convertT, Converter<string, U> convertU,
                char[] split1, char[] split2)
        {
            if (toSplit == null)
                throw new ArgumentNullException("toSplit", "toSplit cannot be null.");

            List<KeyValuePair<T, U>> newList = new List<KeyValuePair<T, U>>();

            string[] pairs = toSplit.Split(split1, StringSplitOptions.RemoveEmptyEntries);

            if (pairs.Length == 0)
                return newList;

            foreach (string pair in pairs)
            {
                string[] pairSplit = pair.Split(split2);
                string secondParam = pairSplit.Length == 1 ? null : pairSplit[1];
                KeyValuePair<T, U> keyPair =
                    new KeyValuePair<T, U>(convertT(pairSplit[0]), convertU(secondParam));
                newList.Add(keyPair);
            }

            return newList;
        }
        #endregion // SplitOnChars
        #region SplitOnCharsUnique
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="toSplit"></param>
        /// <param name="convertT"></param>
        /// <param name="convertU"></param>
        /// <param name="split1"></param>
        /// <param name="split2"></param>
        /// <returns></returns>
        public static Dictionary<T, U> SplitOnCharsUnique<T, U>(string toSplit,
            Converter<string, T> convertT, Converter<string, U> convertU, char[] split1, char[] split2)
        {
            if (toSplit == null)
                throw new ArgumentNullException("toSplit", "toSplit cannot be null.");

            Dictionary<T, U> newList = new Dictionary<T, U>();

            string[] pairs = toSplit.Split(split1, StringSplitOptions.RemoveEmptyEntries);

            if (pairs.Length == 0)
                return newList;

            foreach (string pair in pairs)
            {
                string[] pairSplit = pair.Split(split2);
                string secondParam = pairSplit.Length == 1 ? null : pairSplit[1];
                newList.Add(convertT(pairSplit[0]), convertU(secondParam));
            }

            return newList;
        }
        #endregion // SplitOnCharsUnique

        #region ConvPassthru
        public static Converter<string, string> ConvPassthru =
            delegate (string input)
            {
                return input;
            };
        #endregion // ConvPassthru
        #region ConvPassthruLowerCase
        public static Converter<string, string> ConvPassthruLowerCase =
            delegate (string input)
            {
                return input.Trim().ToLowerInvariant();
            };
        #endregion // ConvPassthruLowerCase
        #region ConvQParam
        public static Converter<string, string> ConvQParam =
            delegate (string input)
            {
                if (input == null)
                    return null;
                input = input.Trim().ToLower();
                if (!input.StartsWith("q="))
                    return null;
                return input.Substring(2);
            };
        #endregion // ConvQParam
        #region ConvStripSpeechMarks
        public static Converter<string, string> ConvStripSpeechMarks =
            delegate (string input)
            {
                if (input == null)
                    return null;
                if (input == @"""""")
                    return "";
                if (!input.StartsWith(@"""") || !input.EndsWith(@"""") || input.Length < 2)
                    return input;
                return input.Substring(1, input.Length - 2);
            };
        #endregion // ConvStripSpeechMarks
    }
}
