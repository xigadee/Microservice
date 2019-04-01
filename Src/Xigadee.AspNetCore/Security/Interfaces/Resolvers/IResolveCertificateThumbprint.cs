namespace Xigadee
{
    /// <summary>
    /// This interface is used to extract the SSL client certificate thumbprint from the incoming request.
    /// This class may need to change if we are using a API gateway in front of the server.
    /// </summary>
    public interface IResolveCertificateThumbprint : IHttpContextResolver<string>
    {
    }
}
