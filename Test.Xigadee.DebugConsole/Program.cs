using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static Context sServerContext;
        static Context sClientContext;

        static Dictionary<string, string> sServerSettings = new Dictionary<string, string>();

        static Dictionary<string, string> sClientSettings = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            sServerContext = new Context();

            var switches = args.CommandArgsParse();

            if (switches.ContainsKey("persistence"))
                sServerContext.SetPersistenceOption(switches["persistence"]);

            if (switches.ContainsKey("persistencecache"))
                sServerContext.SetPersistenceCacheOption(switches["persistencecache"]);

            sServerContext.SlotCount = switches.ContainsKey("processes") ? 
                int.Parse(switches["processes"]) : Environment.ProcessorCount * 4 * 4 * 2;

            sServerContext.EntityVersionid = Guid.NewGuid();
            sServerContext.EntityId = new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");

            sMainMenu.Value.Show(args, 9);
        }

    }
}
