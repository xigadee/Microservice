using System;
using Xigadee;

namespace Test.Xigadee
{

    static partial class Program
    {
        static void ClientConfig(MicroservicePersistenceWrapper<Guid, MondayMorningBlues> wrapper)
        {
            PersistenceClient<Guid, MondayMorningBlues> persistence = null;

            wrapper.Pipeline
                .ConfigurationSetFromConsoleArgs(sSettings.Switches)
                .AddDebugMemoryDataCollector((c) => wrapper.Collector = c)
                .AddChannelIncoming("internalOut")
                    .AttachPersistenceClient(out persistence, "internalIn")
                    .Revert()
                .AddChannelOutgoing("internalIn", internalOnly: true)
                    .Revert();

            wrapper.Persistence = persistence;
        }
    }
}
