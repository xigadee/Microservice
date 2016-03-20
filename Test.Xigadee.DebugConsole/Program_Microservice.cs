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

            sContext.Server.OnRegister += Server_OnRegister;

            sContext.Server.Service.StatusChanged += ServerStatusChanged;

            sContext.Server.Service.StartRequested += ServerStartRequested;
            sContext.Server.Service.StopRequested += ServerStopRequested;

            sContext.Server.Start();
        }

        private static void Server_OnRegister(object sender, CommandRegisterEventArgs e)
        {
            switch (sContext.PersistenceType)
            {
                case PersistenceOptions.Sql:
                    //sContext.Server.Service.RegisterCommand(
                    //    new PersistenceMondayMorningBluesBlob(sContext.Server.Config.Storage));
                    break;
                case PersistenceOptions.Blob:
                    e.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesBlob(e.Config.Storage));
                    break;
                case PersistenceOptions.DocumentDb:
                    e.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesDocDb(e.Config.DocDbCredentials
                        , sContext.Server.Config.DocumentDbName));
                    break;
                case PersistenceOptions.RedisCache:
                    e.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesRedis(e.Config.RedisCacheConnection));
                    break;
            }
        }
    }
}
