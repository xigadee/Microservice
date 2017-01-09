using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class JwtClaimsPrincipal:ClaimsPrincipal
    {
        public JwtClaimsPrincipal(JwtToken token)
        {

        }

    }
}
