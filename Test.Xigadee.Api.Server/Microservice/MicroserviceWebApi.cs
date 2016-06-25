using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Xigadee;

namespace Test.Xigadee.Api.Server
{
    public class MicroserviceWebApi: Microservice
    {
        public MicroserviceWebApi()
        {
            ServicePointManager.DefaultConnectionLimit = 50000;
            StartCompleted += MicroserviceBff_StartCompleted;
            StatisticsIssued += MicroserviceBff_StatisticsIssued;
            StatusChanged += MicroserviceBff_StatusChanged;
        }

        private void MicroserviceBff_StatusChanged(object sender, StatusChangedEventArgs e)
        {

        }

        private void MicroserviceBff_StatisticsIssued(object sender, StatisticsEventArgs e)
        {

        }

        private void MicroserviceBff_StartCompleted(object sender, StartEventArgs e)
        {

        }
    }
}