using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IResourceTracker
    {
        IResourceConsumer RegisterConsumer(string name, ResourceProfile profile);

        IResourceRequestRateLimiter RegisterRequestRateLimiter(string name, IEnumerable<ResourceProfile> profiles);
    }
}
