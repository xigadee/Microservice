using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IClaimsPrincipalUserReferenceResolver
    {
        Task<Guid?> Resolve(ClaimsPrincipal claimsPrincipal);
    }
}
