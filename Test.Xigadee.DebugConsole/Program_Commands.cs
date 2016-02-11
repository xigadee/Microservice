using System;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    partial class Program
    {

        static readonly VersionPolicy<MondayMorningBlues> mVersionBlues =
            new VersionPolicy<MondayMorningBlues>(e => e.VersionId.ToString("N").ToLowerInvariant(), e => e.VersionId = Guid.NewGuid());

        static VersionPolicy<Blah2> mVersionBlah2 =
            new VersionPolicy<Blah2>((e) => e.VersionId.ToString("N").ToLowerInvariant(), (e) => e.VersionId = Guid.NewGuid());


        static IRepositoryAsync<Guid, MondayMorningBlues> sPersistence;

        static IEnumerable<IMessageHandler> CreateReceivers()
        {
            yield return new PersistenceMondayMorningBlues(docDbCreds, docDBdatabase, o => o.Id,  versionMaker: mVersionBlues, resourceProfile: mResourceDocDb)
            {
                ChannelId = "testb"
            };

            //yield return new PersistenceMondayMorningBlues2
            // (
            //     storageCreds
            //     , versionMaker: mVersionBlues
            // )
            //{ ChannelId = "testb" };

            //yield return new PersistenceBlah2
            //(
            //    docDBserver, docDBkey, docDBdatabase
            //    , versionMaker: mVersionBlah2
            //)
            //{
            //    ChannelId = "testb"
            //};
        }

        static void InitialiseJobs(MicroserviceBase service)
        {
            service.RegisterCommand(new TestMasterJob("masterjob"));
            service.RegisterCommand(new TestMasterJob2("masterjob"));
            service.RegisterCommand(new DelayedProcessingJob());
        }

        static void InitialiseInitiators(MicroserviceBase service)
        {
            var initiator1 = new PersistenceMessageInitiator<Guid, MondayMorningBlues>() { ChannelId = "testb", ResponseChannelId = "interserv" };
            var initiator2 = new PersistenceMessageInitiator<Guid, Blah2>() { ChannelId = "testb", ResponseChannelId = "interserv" };

            sPersistence = initiator1;

            service.RegisterCommand(initiator1);
            service.RegisterCommand(initiator2);

        }

        static void InitialisePersistenceSharedService(MicroserviceBase service)
        {
            var initiator = new PersistenceSharedService<Guid, MondayMorningBlues>("internalpersistence") { ChannelId = "testb" };
            service.RegisterCommand(initiator);
            sPersistence = initiator;
        }
    }
}
