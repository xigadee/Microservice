using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This is the client Microservice.
    /// </summary>
    internal class MicroserviceClient: Microservice
    {
        public MicroserviceClient()
        {
            ServicePointManager.DefaultConnectionLimit = 50000;
        }
    }
}
