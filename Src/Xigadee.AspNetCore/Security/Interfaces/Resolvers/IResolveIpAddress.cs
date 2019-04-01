using System.Net;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to set the incoming IPAddress resolver. This class may need to change if we are using a API gateway in front of the server.
    /// </summary>
    public interface IResolveIpAddress : IHttpContextResolver<IPAddress>
    {
    }
}
