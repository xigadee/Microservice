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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This parser is used by both console and service apps to parse incoming parameters
    /// </summary>
    public static class ArgumentsParser
    {
        #region CommandArgsParse(this string[] Args, string strStart, string strDelim, bool throwErrors ...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">The console arguments.</param>
        /// <param name="strStart">The switch start character.</param>
        /// <param name="strDelim">The delimiter character.</param>
        /// <param name="throwErrors">Throws an error if duplicate keys are found or the values are in an incorrect format.</param>
        /// <param name="include">This is an optional function that can be used to filter out specific entries.</param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> CommandArgsParse(this string[] args, string strStart= @"/", string strDelim= @":", bool throwErrors = false, Func<string,string, bool> include = null)
        {
            //	This function parses the command line arguments to find the correct type
            //	based on the syntax /strOption:[strReturnData]
            //	Although there are more efficent ways, other than a for-next loop,
            //	to search through a list, there will only be a limited number of 
            //	items so this has not been optimized. 

            Dictionary<string, string> data = new Dictionary<string, string>();

            if (args == null)
                return data;

            if (include == null)
                include = (k,v) => true;

            string strKey, strValue;

            foreach (string strData in args)
            {
                try
                {
                    ParseData(strData, out strKey, out strValue, strStart, strDelim);

                    if (!data.ContainsKey(strKey) && include (strKey, strValue))
                    {
                        data.Add(strKey, strValue);
                    }
                    else
                    {
                        if (throwErrors)
                            throw new ArgumentException("Multiple keys found.", strKey);
                    }
                }
                catch
                {
                    if (throwErrors)
                    {
                        //Check the string format
                        throw new ArgumentException("Incorrect format", strData);
                    }
                }


            }

            return data;

        }
        #endregion 

        private static void ParseData(string strData, out string strKey,
            out string strValue, string strStart, string strDelim)
        {
            //Ok, trim any space of the data
            strData = strData.Trim();
            //Does the string start with strStart, if not throw an error.
            if (!strData.StartsWith(strStart)) throw new ArgumentException();

            //Is the delimiter of 0 length
            if (strDelim.Length == 0)
            {
                //Just return the key
                strKey = strData.Substring(strStart.Length - 1);
                strValue = "";
            }
            else
            {
                //OK, get the position of the delimiter
                int intDelim = strData.IndexOf(strDelim, strStart.Length);

                if (intDelim == -1)
                {
                    strKey = strData.Substring(strStart.Length);
                    strValue = "";
                }
                else
                {
                    strKey = strData.Substring(strStart.Length, intDelim - strStart.Length);
                    strValue = strData.Substring(intDelim + 1);
                }
            }


        }
    }
}
