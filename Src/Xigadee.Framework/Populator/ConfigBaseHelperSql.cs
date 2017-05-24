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
    /// This helper class is used to provide shortcuts for Sql based services. 
    /// </summary>
    public static class ConfigBaseHelperSql
    {
        /// <summary>
        /// This is the Sql connection string key.
        /// </summary>
        public const string KeySqlConnection = "SqlConnection";
        /// <summary>
        /// This extension method returns the Sql connection string from the config settings.
        /// </summary>
        /// <param name="config">The config class.</param>
        /// <returns>Returns the Sql connection string.</returns>
        public static string SqlConnection(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeySqlConnection);

    }
}
