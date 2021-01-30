﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using Tests.Xigadee.AspNetCore50.Server;
using Tests.Xigadee.AspNetCore50;

namespace Tests.Tpjr.Microservice
{

    /// <summary>
    /// This is the root class used to create the in-memory connection.
    /// </summary>
    public class ProcessingApiTestBase : TestHostBase<TestHostApiConnector, Startup>
    {

    }
}
