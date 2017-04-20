using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ICommandInitiator
    {
        Task<ResponseWrapper<RS>> Process<I, RQ, RS>(RQ rq, RequestSettings settings = null, ProcessOptions? routing = default(ProcessOptions?), IPrincipal principal = null) where I : IMessageContract;
        Task<ResponseWrapper<RS>> Process<RQ, RS>(string channelId, string messageType, string actionType, RQ rq, RequestSettings rqSettings = null, ProcessOptions? routingOptions = default(ProcessOptions?), Func<TaskStatus, TransmissionPayload, bool, ResponseWrapper<RS>> processResponse = null, IPrincipal principal = null);
    }
}