using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Xigadee
{
    /// <summary>
    /// This model is used to exchange registration information.
    /// </summary>
    public class RegistrationModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the customer's preferred name to be displayed.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The account password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The account password confirmation.
        /// </summary>
        public string PasswordConfirm { get; set; }
    }
}
