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
    public class SecurityApiConnector : ApiProviderBase
    {
        /// <summary>
        /// This is the parent constructor. This is used when creating a child container that inherits the parent security settings.
        /// </summary>
        /// <param name="context">The parent connection context</param>
        /// <param name="apiBase">The parent connection context</param>
        protected internal SecurityApiConnector(ConnectionContext context, string apiBase) : base(context)
        {
            cnApiBase = apiBase ?? throw new ArgumentNullException(nameof(apiBase));
        }

        #region Api Shortcuts
        private string cnApiBase { get; set; }

        private string cnApiSessionCreate => $"{cnApiBase}/sessioncreate";
        private string cnApiSessionEnd => $"{cnApiBase}/sessionend";

        private string cnApiSessionLogon => $"{cnApiBase}/logon";

        private string cnApiSessionLogonMsal => $"{cnApiBase}/logon/msal";

        private string cnApiSessionLogoff => $"{cnApiBase}/logoff";

        private string cnApiSessionConfirm2FA => $"{cnApiBase}/c2fa";

        private string cnApiUserPasswordChange => $"{cnApiBase}/passwordchange";

        private string cnApiSessionUserResetRequest => $"{cnApiBase}/userresetrequest";
        private string cnApiSessionUserResetComplete => $"{cnApiBase}/userresetcomplete";
        private string cnApiSessionUserResetCompleteActive => $"{cnApiBase}/userresetcompleteactive";
        private string cnApiSessionUserResetCompleteCode => $"{cnApiBase}/userresetcompletecode";

        private string cnApiUserEmailChange => $"{cnApiBase}/emailchange";
        private string cnApiUserEmailChangeComplete => $"{cnApiBase}/emailchangecomplete";

        private string cnApiSessionNoop => $"{cnApiBase}/sessionnoop";
        private string cnApiSessionCheck => $"{cnApiBase}/sessioncheck";
        #endregion

    }
}
