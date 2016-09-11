#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the standard HTTP response codes that is provided to 
    /// keep consistency between the various persistence implementations. 
    /// </summary>
    public enum PersistenceResponse:int
    {
        Ok200 = 200,
        Created201 = 201,
        Accepted202 = 202,

        BadRequest400 = 400,
        NotFound404 = 404,
        RequestTimeout408 = 408,
        Conflict409 = 409,
        PreconditionFailed412 = 412,
        TooManyRequests429 = 429,

        UnknownError500 = 500,
        NotImplemented501 = 501,
        ServiceUnavailable503 = 503,
        GatewayTimeout504 = 504
    }
}
