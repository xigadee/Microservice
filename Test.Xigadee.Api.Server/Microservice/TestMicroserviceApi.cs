#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.Azure;
using Microsoft.Practices.Unity;
using Xigadee;

#endregion
namespace Test.Xigadee.Api.Server
{
    public class TestMicroserviceApi:Microservice
    {

        public TestMicroserviceApi()
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