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

    }
}
