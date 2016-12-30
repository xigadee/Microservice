#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
            Events.StartCompleted += MicroserviceBff_StartCompleted;
            Events.StatisticsIssued += MicroserviceBff_StatisticsIssued;
            Events.StatusChanged += MicroserviceBff_StatusChanged;
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