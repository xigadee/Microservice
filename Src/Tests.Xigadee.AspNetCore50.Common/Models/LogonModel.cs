using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Xigadee.AspNetCore50
{
    /// <summary>
    /// This is the security information passed to a sign-on method.
    /// </summary>
    public class LogonModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the associated password.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the not a robot confirmation code.
        /// </summary>
        public string NotARobot { get; set; }
    }
}
