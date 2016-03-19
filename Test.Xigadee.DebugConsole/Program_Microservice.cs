using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static void InitialiseMicroserviceClient()
        {

            sContext.Persistence = sContext.Client.Persistence;

            sContext.Client.Service.StatusChanged += ClientStatusChanged;

            sContext.Client.Start();
        }

        static void InitialiseMicroserviceServer()
        {
            sContext.Persistence = sContext.Server.Persistence;

            switch (sContext.PersistenceType)
            {
                case PersistenceOptions.Sql:
                    //sContext.Server.Service.RegisterCommand(
                    //    new PersistenceMondayMorningBluesBlob(sContext.Server.Config.Storage));
                    break;
                case PersistenceOptions.Blob:
                    sContext.Server.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesBlob(sContext.Server.Config.Storage));
                    break;
                case PersistenceOptions.DocumentDb:
                    sContext.Server.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesDocDb(sContext.Server.Config.DocDbCredentials
                        , sContext.Server.Config.DocumentDbName));
                    break;
                case PersistenceOptions.RedisCache:
                    sContext.Server.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesRedis(sContext.Server.Config.RedisCacheConnection));
                    break;
            }

            sContext.Server.Service.StatusChanged += ServerStatusChanged;

            sContext.Server.Service.StartRequested += ServerStartRequested;
            sContext.Server.Service.StopRequested += ServerStopRequested;

            sContext.Server.Start();
        }
    }
}
