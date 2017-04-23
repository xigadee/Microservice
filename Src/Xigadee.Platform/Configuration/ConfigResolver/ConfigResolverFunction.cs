using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the function resolver.
    /// </summary>
    public class ConfigResolverFunction: ConfigResolver
    {
        private Func<string, string, string> mResolver;

        public ConfigResolverFunction(Func<string, string, string> resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("resolver cannot be null");
            mResolver = resolver;
        }

        public override bool CanResolve(string key)
        {
            return mResolver(key, null) != null;
        }

        public override string Resolve(string key)
        {
            return mResolver(key, null);
        }
    }
}
