using System;
using System.Collections.Generic;
using System.Text;

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

        #region SplitOnChars
        /// <summary>
        /// This method is used to split string pairs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="toSplit"></param>
        /// <param name="convertT"></param>
        /// <param name="convertU"></param>
        /// <param name="split1">The initial split character.</param>
        /// <param name="split2">The second split character.</param>
        /// <param name="adjust">The adjust function for the split raw string.</param>
        /// <returns></returns>
        public static List<KeyValuePair<T, U>> SplitOnChars<T, U>(string toSplit,
            Func<string, T> convertT, Func<string, U> convertU,
                char[] split1, char[] split2, Func<string,string> adjust = null)
        {
            if (toSplit == null)
                throw new ArgumentNullException("toSplit", "toSplit cannot be null.");

            List<KeyValuePair<T, U>> newList = new List<KeyValuePair<T, U>>();

            string[] pairs = toSplit.Split(split1, StringSplitOptions.RemoveEmptyEntries);

            if (pairs.Length == 0)
                return newList;

            foreach (string p in pairs)
            {
                var pair = adjust?.Invoke(p)??p;

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
            Func<string, T> convertT, Func<string, U> convertU, char[] split1, char[] split2)
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
        public static Func<string, string> ConvPassthru =
            (input) => input;

        #endregion // ConvPassthru
        #region ConvPassthruLowerCase
        public static Func<string, string> ConvPassthruLowerCase =
            (input) =>
            {
                return input.Trim().ToLowerInvariant();
            };
        #endregion // ConvPassthruLowerCase
        #region ConvQParam
        public static Func<string, string> ConvQParam =
            (input) =>
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
        public static Func<string, string> ConvStripSpeechMarks =
            (input) =>
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

        /// <summary>
        /// This helper method scans through a StringBuilder object to check whether it contains the particular character.
        /// </summary>
        /// <param name="sb">The string builder object.</param>
        /// <param name="c">The character to check.</param>
        /// <returns>Returns true if the string builder contains the character.</returns>
        public static bool ContainsChar(this StringBuilder sb, char c)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == c)
                    return true;
            }

            return false;
        }
    }
}
