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
using Microsoft.Azure.ServiceBus;
using System;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class provides a temporary replacement for the missing SAS-based management functionality 
    /// in the new .NET Standard libraries.
    /// Thanks to Ruppert Koch for code examples at https://code.msdn.microsoft.com/Service-Bus-HTTP-client-fe7da74a
    /// </summary>
    public class NamespaceManagerREST
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManagerREST"/> class.
        /// </summary>
        /// <param name="connection">The Service Bus SAS connection builder object.</param>
        public NamespaceManagerREST(ServiceBusConnectionStringBuilder connection)
        {
            Connection = connection;
        }

        #region Connection
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        public ServiceBusConnectionStringBuilder Connection { get; }
        #endregion
    }
}
