using System;
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

            if (switches.ContainsKey("persistence"))
                sContext.SetPersistenceOption(switches["persistence"]);

            sContext.SlotCount = switches.ContainsKey("processes") ? 
                int.Parse(switches["processes"]) : Environment.ProcessorCount * 4 * 4 * 2;

            sContext.EntityVersionid = Guid.NewGuid();
            sContext.EntityId = new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");

            sMainMenu.Value.Show(args, 9);
        }

    }
}
