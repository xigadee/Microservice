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
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the root documentdb connection object.
    /// </summary>
    public class DocumentDbConnection
    {
        public const string sBase = "https://{0}.documents.azure.com";

        /// <summary>
        /// This is the default constructor for the DocumentDb connection.
        /// </summary>
        /// <param name="account">The acoount name.</param>
        /// <param name="base64key">The accountkey as a base64 encoded key.</param>
        public DocumentDbConnection(string account, string base64key)
        {
            AccountName = account;
            Account = new Uri(string.Format(sBase, AccountName));
            AccountKey = base64key;
            Key = Convert.FromBase64String(base64key);
        }

        /// <summary>
        /// The documentdb account name
        /// </summary>
        public string AccountName { get; private set; }
        /// <summary>
        /// The account uri.
        /// </summary>
        public Uri Account { get; private set; }
        /// <summary>
        /// This is the account key.
        /// </summary>
        public string AccountKey { get; private set; }
        /// <summary>
        /// The binary key.
        /// </summary>
        public byte[] Key { get; private set; }

        public static DocumentDbConnection ToConnection(string account, string base64key)
        {
            return new DocumentDbConnection(account, base64key);
        }
    }
}
