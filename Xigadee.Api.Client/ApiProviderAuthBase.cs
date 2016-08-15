using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class ApiProviderAuthBase
    {


        public abstract void ProcessRequest(HttpRequestMessage rq);

        public abstract void ProcessResponse(HttpResponseMessage rq);


    }

    public class ApiProviderAuthNone: ApiProviderAuthBase
    {
        public override void ProcessRequest(HttpRequestMessage rq)
        {
        }

        public override void ProcessResponse(HttpResponseMessage rq)
        {
        }
    }

    public class ApiProviderAuthApim: ApiProviderAuthBase
    {

        #region ApiKey
        /// <summary>
        /// The Azure api subscription key
        /// </summary>
        public string ApiKey { get; set; }
        #endregion        
        #region ApiTrace
        /// <summary>
        /// Set this to true to initiate an API trace event.
        /// </summary>
        public bool ApiTrace { get; set; }
        #endregion

        public override void ProcessRequest(HttpRequestMessage rq)
        {
            //Add the azure management key when provided.
            if (!string.IsNullOrEmpty(ApiKey))
                rq.Headers.Add(ApimConstants.AzureSubscriptionKeyHeader, ApiKey);

            if (ApiTrace)
                rq.Headers.Add(ApimConstants.AzureTraceHeader, "true");
        }

        public override void ProcessResponse(HttpResponseMessage rq)
        {
        }
    }
}
