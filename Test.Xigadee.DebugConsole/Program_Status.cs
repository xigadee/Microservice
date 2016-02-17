using Xigadee;
using Xigadee;

namespace Test.Xigadee
{
    partial class Program
    {

        static int serverStatus = 0;

        static int clientStatus = 0;

        static void ServerStatusChanged(object sender, StatusChangedEventArgs e)
        {
            ServiceStatusChanged(ref serverStatus, sender, e);
        }

        static void ClientStatusChanged(object sender, StatusChangedEventArgs e)
        {
            ServiceStatusChanged(ref clientStatus, sender, e);
        }

        static void ServiceStatusChanged(ref int started, object sender, StatusChangedEventArgs e)
        {
            var serv = sender as Microservice;

            switch (e.StatusNew)
            {
                case ServiceStatus.Running:
                    started = 2;
                    break;
                case ServiceStatus.Stopped:
                    started = 0;
                    break;
                default:
                    started = 1;
                    break;
            }

            sMainMenu.AddInfoMessage(string.Format("{0}={1}", serv.Statistics.Name, e.StatusNew.ToString()), true);
        }
    }
}
