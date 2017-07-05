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
        /// This method is used to send requests to a remote command and wait for a response.
        /// </summary>
        /// <typeparam name="I">The contract interface.</typeparam>
        /// <typeparam name="RQ">The request type.</typeparam>
        /// <typeparam name="RS">The response type.</typeparam>
        /// <param name="rq">The request object.</param>
        /// <param name="settings">The request settings. Use this to specifically set the timeout parameters, amongst other settings, or to pass meta data to the calling party.</param>
        /// <param name="routing">The routing options by default this will try internal and then external endpoints.</param>
        /// <param name="principal">This is the principal that you wish the command to be executed under. 
        /// By default this is taken from the calling thread if not passed.</param>
        /// <returns>Returns the async response wrapper.</returns>
        Task<ResponseWrapper<RS>> Process<I, RQ, RS>(
              RQ rq
            , RequestSettings settings = null
            , ProcessOptions? routing = default(ProcessOptions?)
            , IPrincipal principal = null
            ) where I : IMessageContract;

        /// <summary>
        /// This method is used to send requests to a remote command and wait for a response.
        /// </summary>
        /// <typeparam name="RQ">The request type.</typeparam>
        /// <typeparam name="RS">The response type.</typeparam>
        /// <param name="channelId"></param>
        /// <param name="messageType"></param>
        /// <param name="actionType"></param>
        /// <param name="rq">The request object.</param>
        /// <param name="settings">The request settings. Use this to specifically set the timeout parameters, amongst other settings, or to pass meta data to the calling party.</param>
        /// <param name="routing">The routing options by default this will try internal and then external endpoints.</param>
        /// <param name="principal">This is the principal that you wish the command to be executed under. 
        /// By default this is taken from the calling thread if not passed.</param>
        /// <returns>Returns the async response wrapper.</returns>
        Task<ResponseWrapper<RS>> Process<RQ, RS>(
              string channelId, string messageType, string actionType
            , RQ rq
            , RequestSettings settings = null
            , ProcessOptions? routing = default(ProcessOptions?)
            , IPrincipal principal = null

            );

        /// <summary>
        /// This method is used to send requests to a remote command and wait for a response.
        /// </summary>
        /// <typeparam name="RQ">The request type.</typeparam>
        /// <typeparam name="RS">The response type.</typeparam>
        /// <param name="header">The message header object that defines the remote endpoint.</param>
        /// <param name="rq">The request object.</param>
        /// <param name="settings">The request settings. Use this to specifically set the timeout parameters, amongst other settings, or to pass meta data to the calling party.</param>
        /// <param name="routing">The routing options by default this will try internal and then external endpoints.</param>
        /// <param name="principal">This is the principal that you wish the command to be executed under. 
        /// By default this is taken from the calling thread if not passed.</param>
        /// <returns>Returns the async response wrapper.</returns>
        Task<ResponseWrapper<RS>> Process<RQ, RS>(
              ServiceMessageHeader header
            , RQ rq
            , RequestSettings settings = null
            , ProcessOptions? routing = default(ProcessOptions?)
            , IPrincipal principal = null
            );
    }
}