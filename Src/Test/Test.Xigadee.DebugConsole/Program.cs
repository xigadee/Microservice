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
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static ConsoleContext sContext;

        static MicroservicePersistenceWrapper<Guid, MondayMorningBlues> sClient;
        static MicroservicePersistenceWrapper<Guid, MondayMorningBlues> sServer;
        static ApiWrapper<Guid, MondayMorningBlues> sApiServer;

        static void Main(string[] args)
        {
            sContext = new ConsoleContext(args);

            sClient = new MicroservicePersistenceWrapper<Guid, MondayMorningBlues>("TestClient", ClientConfig);
            sServer = new MicroservicePersistenceWrapper<Guid, MondayMorningBlues>("TestServer", ServerConfig, ServerInit);
            sApiServer = new ApiWrapper<Guid, MondayMorningBlues>();
            
            //Attach the client events.
            sClient.StatusChanged += StatusChanged;
            sServer.StatusChanged += StatusChanged;
            sApiServer.StatusChanged += StatusChanged;

            //Show the main console menu.
            sMenuMain.Value.Show(args, shortcut: sContext.Shortcut);

            //Detach the client events to allow the application to close.
            sClient.StatusChanged -= StatusChanged;
            sServer.StatusChanged -= StatusChanged;
            sApiServer.StatusChanged -= StatusChanged;

        }

        static void StatusChanged(object sender, StatusChangedEventArgs e)
        {
            var serv = sender as IMicroservice;

            sMenuMain.Value.AddInfoMessage($"{serv.Id.Name}={e.StatusNew.ToString()}{e.Message}", true);
        }
    }
}
