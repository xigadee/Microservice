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

using Xigadee;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sMenuClientServerCommunication = new Lazy<ConsoleMenu>(
    () =>
    {
        var menu = new ConsoleMenu(
            "Communication options"
            , new ConsoleOption("Local connectivity"
                , (m, o) =>
                {
                    sContext.CommunicationType = CommunicationOptions.Local;
                }
                , enabled: (m, o) => true
                , selected: (m, o) => sContext.CommunicationType == CommunicationOptions.Local
            )
            , new ConsoleOption("Azure Service Bus"
                , (m, o) =>
                {
                    sContext.CommunicationType = CommunicationOptions.AzureServiceBus;
                }
                , enabled: (m, o) => sServer.Config?.CanResolve(AzureExtensionMethods.KeyServiceBusConnection) ?? false
                , selected: (m, o) => sContext.CommunicationType == CommunicationOptions.AzureServiceBus
            )
            , new ConsoleOption("Azure Storage Queue"
                , (m, o) =>
                {
                    sContext.CommunicationType = CommunicationOptions.AzureBlobQueue;
                }
                , enabled: (m, o) => sServer.Config?.CanResolve(AzureExtensionMethods.KeyAzureStorageAccountName, AzureExtensionMethods.KeyAzureStorageAccountAccessKey) ?? false
                , selected: (m, o) => sContext.CommunicationType == CommunicationOptions.AzureBlobQueue
            )
            );

        menu.ContextInfoInherit = false;
        menu.AddInfoMessage("Note: Communication options are disabled if config settings are not present.");

        return menu;

    });

    }
}
