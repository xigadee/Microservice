using System;

namespace Xigadee
{
    public interface IPopulator
    {
        Microservice Service { get; }

        void Populate(Func<string, string, string> resolver = null, bool resolverFirst = false);

        void Start();

        void Stop();
    }
}