using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class is used to store an async action for a user account, such as account creation confirmation, and/or 
    /// password reset completion.
    /// </summary>
    public class UserExternalAction: UserReferenceStateBase
    {
        /// <summary>
        /// Gets or sets the type of the action, i.e. Email, SMS, Push etc.
        /// </summary>
        [EntityPropertyHint("typeaction")]
        public string ActionType { get; set; }

        /// <summary>
        /// This property is used to specify a particular communication template, i.e. Registration confirmation, password reset, etc.
        /// </summary>
        [EntityPropertyHint("typetemplate")]
        public string TemplateType { get; set; }

        /// <summary>
        /// This property holds the current state of the action.
        /// </summary>
        [EntityPropertyHint("state")]
        public override string State { get => base.State; set => base.State = value; }

        /// <summary>
        /// Gets or sets the option expiry date/time for the action.
        /// </summary>
        public DateTime? DateExpiry { get; set; }

    }
}
