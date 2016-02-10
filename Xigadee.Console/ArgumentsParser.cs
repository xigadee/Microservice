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
        #region CommandArgsParse(this string[] Args)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> CommandArgsParse(this string[] Args)
        {
            return CommandArgsParse(Args, false);
        }
        #endregion // CommandArgsParse(string[] Args)
        #region CommandArgsParse(this string[] Args, bool throwErrors)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> CommandArgsParse(this string[] Args, bool throwErrors)
        {
            return CommandArgsParse(Args, @"/", @":", throwErrors);
        }
        #endregion // CommandArgsParse(string[] Args, bool throwErrors)
        #region CommandArgsParse(this string[] Args, string strStart, bool throwErrors)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="strStart"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> CommandArgsParse(string[] Args, string strStart, bool throwErrors)
        {
            return CommandArgsParse(Args, strStart, @":", throwErrors);
        }
        #endregion // CommandArgsParse(string[] Args, string strStart, bool throwErrors)
        #region CommandArgsParse(this string[] Args, string strStart, string strDelim, bool throwErrors)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="strStart"></param>
        /// <param name="strDelim"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> CommandArgsParse(this string[] Args, string strStart, string strDelim, bool throwErrors)
        {
            //	This function parses the command line arguments to find the correct type
            //	based on the syntax /strOption:[strReturnData]
            //	Although there are more efficent ways, other than a for-next loop,
            //	to search through a list, there will only be a limited number of 
            //	items so this has not been optimized. 

            Dictionary<string, string> data = new Dictionary<string, string>();

            if (Args == null)
                return data;

            string strKey, strValue;

            foreach (string strData in Args)
            {
                try
                {
                    ParseData(strData, out strKey, out strValue, strStart, strDelim);

                    if (!data.ContainsKey(strKey))
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
