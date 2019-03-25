using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class is used to store an async action for a user account, such as account creation confirmation, and/or 
    /// password reset completion.
    /// </summary>
    public abstract class UserExternalActionBase: UserSecondaryBase
    {
        /// <summary>
        /// Gets or sets the type of the action, i.e. Registration confirmation, password reset, etc.
        /// </summary>
        public string ActionType { get; set; }

    }
}
