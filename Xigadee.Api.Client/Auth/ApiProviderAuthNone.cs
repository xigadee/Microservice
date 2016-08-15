using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ApiProviderAuthNone: IApiProviderAuthBase
    {
        public void ProcessRequest(HttpRequestMessage rq)
        {
        }

        public void ProcessResponse(HttpResponseMessage rq)
        {
        }
    }
}
