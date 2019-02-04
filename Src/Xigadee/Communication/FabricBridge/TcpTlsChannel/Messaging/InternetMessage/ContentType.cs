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
    /// <summary>
    /// This structure processes the content type field and extracts the media type
    /// and the parameters.
    /// </summary>
    public class ContentType
    {
        public string MediaType = null;
        public Dictionary<string, string> Parameters = null;

        public ContentType(string data)
        {
            string[] items = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            MediaType = items[0].TrimStart().ToLowerInvariant();

            if (items.Length <= 1)
                return;

            Parameters = new Dictionary<string, string>(items.Length - 1);
            int count = 1;
            while (count < items.Length)
            {
                string item = items[count];
                int pos = item.IndexOf('=');
                if (pos == -1)
                    Parameters.Add(item.ToLowerInvariant().TrimStart(), null);
                else
                    Parameters.Add(
                        item.Substring(0, pos).ToLowerInvariant().TrimStart()
                        , item.Substring(pos + 1));

                count++;
            }
        }

        public string Parameter(string id)
        {
            if (Parameters == null || !Parameters.ContainsKey(id))
                return null;

            return Parameters[id];
        }
    }
}
