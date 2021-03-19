using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This is the root class used to create the in-memory connection.
    /// </summary>
    public abstract class TestHostBase<S>
        where S : class
    {
        #region Declarations
        protected HttpClient _client;
        protected TestServer _server;
        #endregion

        #region SetupServer()
        /// <summary>
        /// Sets up this instance.
        /// </summary>
        public virtual void ServerSetup()
        {
            if (_server == null)
            {
                _server = new TestServer(new WebHostBuilder().UseStartup<S>());
                _client = _server.CreateClient();
            }
        }
        #endregion
        #region CleanUpServer()
        /// <summary>
        /// Cleans up and disposes of the instance.
        /// </summary>
        public virtual void ServerCleanUp()
        {
            if (_server != null)
            {
                _client.Dispose();
                _server.Dispose();
            }
        }
        #endregion

        #region ConnectorSet(ApiProviderBase connector)
        /// <summary>
        /// This method is used to set the client override if the uri is not null.
        /// If the Uri is null, this will also start the server locally.
        /// </summary>
        /// <param name="uri">The uri to check.</param>
        /// <param name="connector">The connector to populate the client.</param>
        protected void ConnectorSet(ApiProviderBase connector)
        {
            //Do not override if the Uri has been set.
            ServerSetup();
            connector.ClientOverride = _client;
        }
        #endregion
    }

    /// <summary>
    /// This extended test class includes the code to create the connector.
    /// </summary>
    /// <typeparam name="C">The connector type.</typeparam>
    /// <typeparam name="S">The startup class.</typeparam>
    public abstract class TestHostBase<C, S> : TestHostBase<S>
        where S : class
        where C : ApiProviderBase, new()
    {
        #region ConnectorGet ...
        /// <summary>
        /// This method pulls out the parameters for the environment from the Test Context and tries to logon.
        /// </summary>
        /// <param name="type">The environment type.</param>
        /// <returns>Returns the connector.</returns>
        protected C ConnectorGetLocal()
        {
            var Connector = new C();

            //Do not override if the Uri has been set.
            ConnectorSet(Connector);

            return Connector;
        }
        #endregion


    }
}
