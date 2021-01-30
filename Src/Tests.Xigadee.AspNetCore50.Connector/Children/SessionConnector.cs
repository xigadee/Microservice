using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50.Connector
{
    /// <summary>
    /// This class is used to provide common security support between the various Api connectors.
    /// </summary>
    public class SessionApiConnector : ApiProviderBase
    {
        /// <summary>
        /// This is the parent constructor. This is used when creating a child container that inherits the parent security settings.
        /// </summary>
        /// <param name="context">The parent connection context</param>
        /// <param name="apiBase">The parent connection context</param>
        protected internal SessionApiConnector(ConnectionContext context, string apiBase) : base(context)
        {
            cnApiBase = apiBase ?? throw new ArgumentNullException(nameof(apiBase));
        }

        #region Api Shortcuts
        private string cnApiBase { get; set; }

        private string cnApiSessionCreate => $"{cnApiBase}/sessioncreate";
        private string cnApiSessionEnd => $"{cnApiBase}/sessionend";


        private string cnApiSessionLogoff => $"{cnApiBase}/logoff";


        private string cnApiSessionNoop => $"{cnApiBase}/sessionnoop";
        private string cnApiSessionCheck => $"{cnApiBase}/sessioncheck";
        #endregion

        #region SessionStart()
        /// <summary>
        /// This method starts a session.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SessionStart()
        {
            var rs = await SessionStartExtended();

            return rs.IsSuccess;
        }

        public async Task<ApiResponse<JwtResponseModel>> SessionStartExtended()
        {
            var uri = UriBaseAppend(cnApiSessionCreate);
            var rs = await CallApiClient<JwtResponseModel>(HttpMethod.Post, uri);

            if (rs.IsSuccess)
            {
                if (!string.IsNullOrEmpty(rs.Entity?.Token))
                {
                    Context.AuthHandlers.Clear();
                    Context.AuthHandlers.Add(new JwtAuthProvider(rs.Entity.Token));
                }
            }

            return rs;
        }
        #endregion
        #region SessionEnd()
        /// <summary>
        /// This method is used to logoff a session.
        /// </summary>
        /// <returns>Returns true if success.</returns>
        public Task<ApiResponse> SessionEnd() =>
            CallApiClient(HttpMethod.Post, UriBaseAppend(cnApiSessionEnd));
        #endregion

        #region SessionLogoff()
        /// <summary>
        /// This method is used to logoff a session.
        /// </summary>
        /// <returns>Returns true if success.</returns>
        public Task<ApiResponse> SessionLogoff() =>
            CallApiClient(HttpMethod.Post, UriBaseAppend(cnApiSessionLogoff));

        #endregion

        #region SessionNoop()
        /// <summary>
        /// This method is used to check a connection is active.
        /// </summary>
        /// <returns>Returns true if success.</returns>
        public Task<ApiResponse> SessionNoop() =>
            CallApiClient(HttpMethod.Get, UriBaseAppend(cnApiSessionNoop));

        #endregion
        #region SessionCheck()
        /// <summary>
        /// This method is used to check a session is active.
        /// </summary>
        /// <returns>Returns true if success.</returns>
        public Task<ApiResponse> SessionCheck() =>
            CallApiClient(HttpMethod.Get, UriBaseAppend(cnApiSessionCheck));

        #endregion

        #region SessionValidate()
        /// <summary>
        /// This method quickly validates the current state of the connection.
        /// </summary>
        /// <returns>Returns a tuple indicating the state of the connection.</returns>
        public async Task<(bool sessionOK, bool sessionAuthOk)> SessionValidate()
        {
            var rs1 = await SessionNoop();
            var rs2 = await SessionCheck();

            return (rs1.IsSuccess, rs2.IsSuccess);
        }
        #endregion
    }
}
