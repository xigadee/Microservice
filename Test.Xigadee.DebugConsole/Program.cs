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

            sContext.ApiUri = new Uri("http://localhost:29001");

            var switches = args.CommandArgsParse();

            if (switches.ContainsKey("persistence"))
                sContext.SetServicePersistenceOption(switches["persistence"]);

            if (switches.ContainsKey("persistencecache"))
                sContext.SetServicePersistenceCacheOption(switches["persistencecache"]);

            sContext.SlotCount = switches.ContainsKey("processes") ? 
                int.Parse(switches["processes"]) : Environment.ProcessorCount * 4 * 4;

            sContext.EntityVersionid = Guid.NewGuid();
            sContext.EntityId = new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");

            sMainMenu.Value.Show(args, shortcut:switches.ContainsKey("shortcut")?switches["shortcut"]:null);
        }

    }
}
