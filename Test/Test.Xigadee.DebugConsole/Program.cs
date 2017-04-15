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

            sClient = new MicroservicePersistenceWrapper<Guid, MondayMorningBlues>("Test client", BuildClient);
            sClient.StatusChanged += StatusChanged;

            sServer = new MicroservicePersistenceWrapper<Guid, MondayMorningBlues>("Test server", BuildServer);
            sServer.StatusChanged += StatusChanged;

            sApiServer = new ApiWrapper<Guid, MondayMorningBlues>();
            sApiServer.StatusChanged += StatusChanged;

            sMenuMain.Value.Show(args, shortcut:sContext.Shortcut);
        }

        static void StatusChanged(object sender, StatusChangedEventArgs e)
        {
            var serv = sender as IConsolePersistence<Guid, MondayMorningBlues>;

            sMenuMain.Value.AddInfoMessage($"{serv.Name}={e.StatusNew.ToString()}{e.Message}", true);
        }


    }
}
