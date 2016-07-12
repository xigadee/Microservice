using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is a default microservice for the web api.
    /// </summary>
    public class MicroserviceWebApi:Microservice
    {

        public MicroserviceWebApi()
        {
            ServicePointManager.DefaultConnectionLimit = 50000;
#if (DEBUG)
            StartCompleted += MicroserviceBff_StartCompleted;
            StatisticsIssued += MicroserviceBff_StatisticsIssued;
            StatusChanged += MicroserviceBff_StatusChanged;
#endif
        }

#if (DEBUG)
        private void MicroserviceBff_StatusChanged(object sender, StatusChangedEventArgs e)
        {

        }

        private void MicroserviceBff_StatisticsIssued(object sender, StatisticsEventArgs e)
        {

        }

        private void MicroserviceBff_StartCompleted(object sender, StartEventArgs e)
        {

        }
#endif
    }
}
