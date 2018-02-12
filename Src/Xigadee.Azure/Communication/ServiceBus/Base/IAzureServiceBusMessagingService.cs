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
using Microsoft.ServiceBus.Messaging;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This interface is used for Azure service bus messaging.
    /// </summary>
    /// <typeparam name="P">The partition config type.</typeparam>
    public interface IAzureServiceBusMessagingService<P>//: IMessagingService<P>
        where P : PartitionConfig
    {
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        AzureConnection AzureConn { get; set; }
    }
}
