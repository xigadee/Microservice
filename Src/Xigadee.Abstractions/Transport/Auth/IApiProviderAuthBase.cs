using System.Net.Http;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to add authentication support to a HttpRequest.
    /// </summary>
    public interface IApiProviderAuthBase
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="rq">The request.</param>
        void ProcessRequest(HttpRequestMessage rq);
        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="rs">The response.</param>
        void ProcessResponse(HttpResponseMessage rs);
    }
}
