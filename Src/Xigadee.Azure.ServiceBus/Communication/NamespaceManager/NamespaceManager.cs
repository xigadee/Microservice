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
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class provides a temporary replacement for the missing functionality in the new .NET Standard libraries.
    /// </summary>
    public class NamespaceManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManager"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public NamespaceManager(AzureServiceBusConnection connection)
        {
            Connection = connection;
        }

        #region Connection
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        public AzureServiceBusConnection Connection { get; }
        #endregion
    }
}
