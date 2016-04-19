using System;
using System.Diagnostics;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static void PersistenceLog(ConsoleMenu menu, string action, bool success)
        {
            menu.AddInfoMessage($"{action} {(success ? "OK" : "Fail")}"
                , true, success ? EventLogEntryType.Information : EventLogEntryType.Error);
        }

        static void ServerStatusChanged(object sender, StatusChangedEventArgs e)
        {
            ServiceStatusChanged((v) => sServerContext.Server.Status = v, sender, e);
        }

        static void ClientStatusChanged(object sender, StatusChangedEventArgs e)
        {
            ServiceStatusChanged((v) => sServerContext.Client.Status = v, sender, e);
        }

        static void ServiceStatusChanged(Action<int> started, object sender, StatusChangedEventArgs e)
        {
            var serv = sender as Microservice;

            switch (e.StatusNew)
            {
                case ServiceStatus.Running:
                    started(2);
                    break;
                case ServiceStatus.Stopped:
                    started(0);
                    break;
                default:
                    started(1);
                    break;
            }

            sMainMenu.Value.AddInfoMessage(string.Format("{0}={1}", serv.Statistics.Name, e.StatusNew.ToString()), true);
        }

        private static void ServerStopRequested(object sender, StopEventArgs e)
        {

        }

        private static void ServerStartRequested(object sender, StartEventArgs e)
        {
            e.ConfigurationOptions.ConcurrentRequestsMax = 4;
            e.ConfigurationOptions.ConcurrentRequestsMin = 1;
            e.ConfigurationOptions.StatusLogFrequency = TimeSpan.FromSeconds(15);

            //e.ConfigurationOptions.OverloadProcessLimit = 2;
            //if (e.ConfigurationOptions.OverloadProcessLimit == 0)
            //    e.ConfigurationOptions.OverloadProcessLimit = 11;        
        }
    }
}
