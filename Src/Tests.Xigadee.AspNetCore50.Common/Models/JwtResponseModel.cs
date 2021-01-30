using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Xigadee.AspNetCore50
{
    /// <summary>
    /// This class is used to send back an easily formatted Jwt token container.
    /// </summary>
    public class JwtResponseModel
    {
        /// <summary>
        /// The token.
        /// </summary>
        public string Token { get; set; }
    }
}
