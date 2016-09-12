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
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static Context sContext;

        static void Main(string[] args)
        {
            sContext = new Context();


            var switches = args.CommandArgsParse();

            if (switches.ContainsKey("apiuri"))
                sContext.ApiServer.ApiUri = new Uri(switches["apiuri"]);
            else
                sContext.ApiServer.ApiUri = new Uri("http://localhost:29001");


            if (switches.ContainsKey("persistence"))
                sContext.SetServicePersistenceOption(switches["persistence"]);

            if (switches.ContainsKey("persistencecache"))
                sContext.SetServicePersistenceCacheOption(switches["persistencecache"]);

            sContext.SlotCount = switches.ContainsKey("processes") ? 
                int.Parse(switches["processes"]) : Environment.ProcessorCount * 4 * 4;

            sContext.EntityVersionid = Guid.NewGuid();
            sContext.EntityId = new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");

            sMenuMain.Value.Show(args, shortcut:switches.ContainsKey("shortcut")?switches["shortcut"]:null);
        }

    }
}
