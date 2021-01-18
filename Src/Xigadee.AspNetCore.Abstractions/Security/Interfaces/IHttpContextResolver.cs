using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to extract information from the incoming HttpContext.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    public interface IHttpContextResolver<T>
    {
        /// <summary>
        /// Resolves the item from the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>Returns the resolved item.</returns>
        Task<T> Resolve(HttpContext httpContext);
    }
}
