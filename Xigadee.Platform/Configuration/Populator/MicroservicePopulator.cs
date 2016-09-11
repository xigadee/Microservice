using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class MicroservicePopulator<C>
        where C:ConfigBase, new()
    {
        public MicroservicePopulator(Microservice service = null, C config = null
            , Func<string, string, string> resolver = null, bool resolverFirst = false)
        {
            Service = service ?? new Microservice();
            Config = config ?? new C();

            if (resolver != null)
            {
                Config.Resolver = resolver;
                Config.ResolverFirst = resolverFirst;
            }
        }

        public Microservice Service { get; }

        public C Config { get; }

    }


}
