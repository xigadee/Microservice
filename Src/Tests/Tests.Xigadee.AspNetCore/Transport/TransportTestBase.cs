using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Xigadee
{
    /// <summary>
    /// This is the root class used to create the in-memory connection.
    /// </summary>
    public abstract class TransportTestBase
    {
        protected HttpClient _client;
        protected TestServer _server;

        /// <summary>
        /// Sets up this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            //_server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>());
            //_client = _server.CreateClient();
        }
        /// <summary>
        /// Cleans up and disposes of the instance.
        /// </summary>
        [TestCleanup]
        public void CleanUp()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}
