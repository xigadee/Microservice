using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IResourceConsumer: IResourceBase
    {
        Guid Start(string group, Guid requestId);

        void End(Guid profileId, int start, ResourceRequestResult result);

        void Retry(Guid profileId, int retryStart, ResourceRetryReason reason);

        void Exception(Guid profileId, int retryStart);
    }
}
