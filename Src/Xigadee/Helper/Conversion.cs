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

        public static double? ToDouble(string data)
        {
            if (data == null)
                return null;

            double val;

            if (double.TryParse(data, out val))
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
