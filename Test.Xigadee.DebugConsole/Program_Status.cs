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
            ServiceStatusChanged((v) => sContext.Server.Status = v, sender, e);
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

            sMenuMain.Value.AddInfoMessage(string.Format("{0}={1}", serv.Name, e.StatusNew.ToString()), true);
        }

    }
}
