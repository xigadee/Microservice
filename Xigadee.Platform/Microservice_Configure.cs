#region using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //TaskManager
    public partial class Microservice
    {
        /// <summary>
        /// This method is used to build a pipeline used to configure the Microservice
        /// </summary>
        /// <typeparam name="C">The config type.</typeparam>
        /// <param name="resolver">The resolver used by the config class to resolve key/value pairs.</param>
        /// <param name="resolverFirst">Specifies whether the resolver should be used first before falling back to the root config.</param>
        /// <param name="action">This is an action that can be used to make changes to the underlying microservice.</param>
        /// <returns>Returns a pipeline that can be used to configure a microservice.</returns>
        public static MicroservicePipeline Configure<C>(
              Func<string, string, string> resolver = null
            , bool resolverFirst = false
            , Action<Microservice> action = null) 
            where C : ConfigBase, new()
        {
            var service = new Microservice();

            C config = new C();
            if (resolver != null)
            {
                config.Resolver = resolver;
                config.ResolverFirst = resolverFirst;
            }

            action?.Invoke(service);

            return new MicroservicePipeline(service, config);
        }

        /// <summary>
        /// This method is used to build a pipeline used to configure the Microservice
        /// </summary>
        /// <param name="resolver">The resolver used by the config class to resolve key/value pairs.</param>
        /// <param name="resolverFirst">Specifies whether the resolver should be used first before falling back to the root config.</param>
        /// <param name="assign">This is an action that can be used to make changes to the underlying microservice.</param>
        /// <returns>Returns a pipeline that can be used to configure a microservice.</returns>
        public static MicroservicePipeline Configure(
              Action<Microservice> assign = null,
              Func<string, string, string> resolver = null
            , bool resolverFirst = false
            )
        {
            return Configure<ConfigBase>(resolver: resolver, resolverFirst: resolverFirst, action: assign);
        }
    }
}
