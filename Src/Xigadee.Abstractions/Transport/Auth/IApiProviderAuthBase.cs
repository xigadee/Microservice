using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{

    public interface IApiProviderAuthBase
    {
        void ProcessRequest(HttpRequestMessage rq);

        void ProcessResponse(HttpResponseMessage rq);
    }
}
