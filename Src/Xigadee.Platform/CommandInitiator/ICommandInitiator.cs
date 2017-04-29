using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the command initiator interface.
    /// </summary>
    public interface ICommandInitiator
    {
        /// <summary>
        /// This is the default request.
        /// </summary>
        /// <typeparam name="I">The message contract interface type.</typeparam>
        /// <typeparam name="RQ">The request type.</typeparam>
        /// <typeparam name="RS">The response type.</typeparam>
        /// <param name="rq">The request.</param>
        /// <param name="settings">The optional request settings object.</param>
        /// <param name="routing">The routing instructions.</param>
        /// <param name="principal">The calling party security principal. If this is not passed, it will be inferred from the thrread.</param>
        /// <returns>Returns the success object and any associated return data object.</returns>
        Task<ResponseWrapper<RS>> Process<I, RQ, RS>(RQ rq
            , RequestSettings settings = null
            , ProcessOptions? routing = default(ProcessOptions?)
            , IPrincipal principal = null
            ) where I : IMessageContract;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="RQ"></typeparam>
        /// <typeparam name="RS"></typeparam>
        /// <param name="channelId"></param>
        /// <param name="messageType"></param>
        /// <param name="actionType"></param>
        /// <param name="rq"></param>
        /// <param name="settings"></param>
        /// <param name="routing"></param>
        /// <param name="processResponse"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        Task<ResponseWrapper<RS>> Process<RQ, RS>(
            string channelId, string messageType, string actionType
            , RQ rq, RequestSettings settings = null
            , ProcessOptions? routing = default(ProcessOptions?)
            , Func<TaskStatus, TransmissionPayload, bool, ResponseWrapper<RS>> processResponse = null
            , IPrincipal principal = null);
    }
}