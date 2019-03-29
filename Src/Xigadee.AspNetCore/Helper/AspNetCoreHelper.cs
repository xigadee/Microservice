using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class contains extension methods to help in the application configuration.
    /// </summary>
    public static class XigadeeAspNetCoreHelper
    {
        /// <summary>
        /// Extracts a service implementation from the service collection.
        /// </summary>
        /// <typeparam name="I">The service type.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="throwErrorOnNotFound">Throw error if service not found</param>
        /// <returns>Returns the instantiated service instance.</returns>
        public static I ServiceExtract<I>(this IServiceCollection services, bool throwErrorOnNotFound = false)
            where I : class
        {
            var service = (services.FirstOrDefault((s) => s.ServiceType == typeof(I)));

            //if (throwErrorOnNotFound && service == null)
            //    throw new ServiceNotFoundInCollectionException(typeof(I).Name);

            return service.ImplementationInstance as I;
        }
    }
}
