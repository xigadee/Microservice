using System;
using System.Text;
using System.Collections.Generic;

namespace Xigadee
{
    public class WWWFormUrlEncodedBodyFragment : InternetMessageFragmentBody
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public WWWFormUrlEncodedBodyFragment()
            : base()
        {
        }
        #endregion

        #region ExtractDataDictionary()
        /// <summary>
        /// This method extracts the URL encoded body data as a string dictionary.
        /// </summary>
        /// <returns>Returns a dictionary containing the data.</returns>
        public virtual Dictionary<string, string> ExtractDataDictionary()
        {
            if (this.Length == 0)
                return new Dictionary<string, string>();

            string UrlData = Encoding.ASCII.GetString(mBuffer, 0, (int)Length);

            Dictionary<string, string> data =
                StringHelper.SplitOnCharsUnique<string, string>(
                  UrlData
                , StringHelper.ConvPassthru
                , delegate(string input)
                {
                    if (input == null)
                        return null;
                    return Uri.UnescapeDataString(input);
                }, new char[] { '&' }, new char[] { '=' });

            return data;
        }
        #endregion // ExtractDataUnique()
        #region ExtractDataList()
        /// <summary>
        /// This method extracts the incoming data as a keyvaluepair. This method allows multiple parameters
        /// of the same key value to be extracted from the data.
        /// </summary>
        /// <returns></returns>
        public virtual List<KeyValuePair<string, string>> ExtractDataList()
        {
            if (this.Length == 0)
                return new List<KeyValuePair<string, string>>();

            string UrlData = Encoding.ASCII.GetString(mBuffer, 0, (int)Length);

            List<KeyValuePair<string, string>> data =
                StringHelper.SplitOnChars<string, string>(UrlData, StringHelper.ConvPassthru,
                delegate(string input)
                {
                    if (input == null)
                        return null;
                    return Uri.UnescapeDataString(input);
                }, new char[] { '&' }, new char[] { '=' });

            return data;
        }
        #endregion // ExtractDataList()
    }
}
