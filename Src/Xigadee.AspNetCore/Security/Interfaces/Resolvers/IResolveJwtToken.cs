namespace Xigadee
{
    /// <summary>
    /// This interface is used to extract the JWT Token from the incoming request.
    /// </summary>
    public interface IResolveJwtToken : IHttpContextResolver<string>
    {
    }
}
