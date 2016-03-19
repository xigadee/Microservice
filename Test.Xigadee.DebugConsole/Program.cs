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
            sContext.Client = new PopulatorClient();
            sContext.Server = new PopulatorServer();

            var id = new Guid("5ac0802f-7768-433c-bc54-975940964363");
            var value = id.ToByteArray();
            int push = (value[0] >> 6) - 1;

            var switches = args.CommandArgsParse();

            sContext.SlotCount = switches.ContainsKey("processes") ? 
                int.Parse(switches["processes"]) : Environment.ProcessorCount * 4 * 4 * 2;

            //var testid = Guid.NewGuid();
            sContext.Versionid = Guid.NewGuid();
            sContext.Testid = new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");
            //var testid = Guid.NewGuid();

            sMainMenu.Value.Show(args, 9);
        }

    }
}
