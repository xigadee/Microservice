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
    public class HeaderFragmentMultiPart : HeaderFragment<MessageTerminatorCRLFFolding>
    {
        #region Declarations
        Dictionary<string, string> mFieldParts = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HeaderFragmentMultiPart()
            : base()
        {
            mFieldParts = new Dictionary<string, string>();
            mField = null;
            mFieldData = null;
        }
        #endregion

        #region MessagePartsBuild()
        /// <summary>
        /// This method breaks the DataString in to its constituent parts.
        /// </summary>
        /// <param name="force"></param>
        protected override void MessagePartsBuild()
        {
            base.MessagePartsBuild();

            if (mFieldData == null)
                return;

            try
            {
                string[] items = mFieldData.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                mFieldData = items[0].TrimStart().ToLowerInvariant();

                if (items.Length <= 1)
                    return;

                //mFieldParts = new Dictionary<string, string>(items.Length - 1);
                int count = 1;
                while (count < items.Length)
                {
                    string item = items[count];
                    int pos = item.IndexOf('=');
                    if (pos == -1)
                        mFieldParts.Add(item.ToLowerInvariant().TrimStart(), null);
                    else
                        mFieldParts.Add(
                            item.Substring(0, pos).ToLowerInvariant().TrimStart()
                            , item.Substring(pos + 1));

                    count++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region EndInitCustom()
        /// <summary>
        /// This override sets the DataString at the end of the message initialization.
        /// </summary>
        protected override void EndInitCustom()
        {
            base.EndInitCustom();
        }
        #endregion // EndInitCustom()

        #region AddMultipart(string key, string value)
        /// <summary>
        /// This method will add a key-value pair to the multipart values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddMultipart(string key, string value)
        {
            if (!Initializing)
                throw new NotSupportedException("Multipart Field cannot be set when the fragment is not initializing.");
            key = key.ToLowerInvariant();

            if (mFieldParts.ContainsKey(key))
                mFieldParts[key] = value;
            else
                mFieldParts.Add(key, value);
        }
        #endregion // AddMultipart(string key, string value)

    }
}
