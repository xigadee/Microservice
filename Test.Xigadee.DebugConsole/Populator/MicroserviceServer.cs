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
    /// This is the server Microservice.
    /// </summary>
    public class MicroserviceServer: Microservice
    {
        public MicroserviceServer()
        {
            ServicePointManager.DefaultConnectionLimit = 50000;
        }
    }
}
