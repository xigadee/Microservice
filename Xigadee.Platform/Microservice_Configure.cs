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
        public static ConfigurationPipeline Configure<C>(Func<string, string, string> resolver = null, bool resolverFirst = false) 
            where C : ConfigBase, new()
        {
            var service = new Microservice();
            C config = new C();
            if (resolver != null)
            {
                config.Resolver = resolver;
                config.ResolverFirst = resolverFirst;
            }

            return new ConfigurationPipeline(service, config);
        }

        public static ConfigurationPipeline Configure(Func<string, string, string> resolver = null, bool resolverFirst = false) 
        {
            return Configure<ConfigBase>(resolver: resolver, resolverFirst: resolverFirst);
        }

    }
}
