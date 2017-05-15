#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

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
