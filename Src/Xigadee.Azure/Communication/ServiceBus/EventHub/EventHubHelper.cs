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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public static class EventHubHelper
    {
        public static EventHubDescription EventHubDescriptionGet(string tPath)
        {
            return new EventHubDescription(tPath);
        }

        /// <summary>
        /// This method creates the queue if it does not exist.
        /// </summary>
        public static void EventHubFabricInitialize(this AzureConnection conn, string name)
        {
            if (conn.NamespaceManager.EventHubExists(name))
                return;

            try
            {
                conn.NamespaceManager.CreateEventHubIfNotExists(EventHubDescriptionGet(name));
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                // Another service created it before we did - just use that one
            }
        }
    }
}
