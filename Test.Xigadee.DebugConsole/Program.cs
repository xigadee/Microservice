using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static PopulatorClient sClient;

        static PopulatorServer sServer;

        static IPopulatorConsole sService;

        static Func<int> mPersistenceStatus = () => 0;

        static int sSlotCount;

        static void Main(string[] args)
        {
            var id = new Guid("5ac0802f-7768-433c-bc54-975940964363");
            var value = id.ToByteArray();
            int push = (value[0] >> 6) - 1;

            var switches = args.CommandArgsParse();

            sState = new PersistenceState();

            sSlotCount = switches.ContainsKey("processes") ? int.Parse(switches["processes"]) : Environment.ProcessorCount * 4 * 4 * 2;

            //var testid = Guid.NewGuid();
            sState.Versionid = Guid.NewGuid();
            sState.Testid = new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");
            //var testid = Guid.NewGuid();

            sMainMenu.Show(args, 9);
        }

        static PersistenceState sState;

        class PersistenceState
        {
            public Guid Versionid;
            public Guid Testid;

        }
    }
}
