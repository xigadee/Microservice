using System;

namespace Xigadee
{
    public interface IPopulator
    {
        MicroserviceBase Service { get; }

        void Populate(Func<string, string, string> resolver = null, bool resolverFirst = false);
    }
}