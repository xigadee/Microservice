using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using Tests.Xigadee.AspNetCore50.Server;

namespace Tests.Xigadee.AspNetCore50
{

    /// <summary>
    /// This is the root class used to create the in-memory connection.
    /// </summary>
    public class TestServerUnitTestBase : TestHostBase<TestHostApiConnector, Startup>
    {

    }
}
