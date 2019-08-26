#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region Events
        /// <summary>
        /// This event is fired when an request is received and resolved.
        /// </summary>
        public event EventHandler<ProcessRequestEventArgs> OnRequest;
        /// <summary>
        /// This event is fired when a request is received but not resolved.
        /// </summary>
        public event EventHandler<ProcessRequestEventArgs> OnRequestUnresolved;
        /// <summary>
        /// This event is fired when a request is received but not resolved.
        /// </summary>
        public event EventHandler<ProcessRequestEventArgs> OnRequestUnhandledException;
        #endregion

        #region --> ProcessRequest(TransmissionPayload payload, List<TransmissionPayload> responses)
        /// <summary>
        /// This method is called to process an incoming payload.
        /// </summary>
        /// <param name="rq">The message to process.</param>
        /// <param name="responses">The return path for the message.</param>
        public virtual async Task ProcessRequest(TransmissionPayload rq, List<TransmissionPayload> responses)
        {
            var header = rq.Message.ToServiceMessageHeader();
            H handler;
            if (!SupportedResolve(header, out handler))
            {
                var ex = new CommandNotSupportedException(rq.Id, header, GetType());
                FireAndDecorateEventArgs(OnRequestUnresolved, () => new ProcessRequestEventArgs(rq, ex));
                throw ex;
            }

            FireAndDecorateEventArgs(OnRequest, ()=> new ProcessRequestEventArgs(rq));

            int start = StatisticsInternal.ActiveIncrement();
            try
            {
                //Call the registered command. This should not throw an exception if a hander has been registered.
                await handler.Execute(rq, responses);
            }
            catch (Exception ex)
            {
                StatisticsInternal.ErrorIncrement();
                FireAndDecorateEventArgs(OnRequestUnhandledException, () => new ProcessRequestEventArgs(rq, ex));
                //Handler has not caught the exception, so we default to the default policy behaviour.
                await ProcessRequestException(ex, rq, responses);
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(start);
            }
        }
        #endregion

        #region ProcessRequestException(Exception ex, TransmissionPayload rq, List<TransmissionPayload> responses)
        /// <summary>
        /// Processes the exception.
        /// </summary>
        /// <param name="ex">The exception raised.</param>
        /// <param name="rq">The incoming request.</param>
        /// <param name="responses">The responses.</param>
        protected virtual Task ProcessRequestException(Exception ex, TransmissionPayload rq, List<TransmissionPayload> responses)
        {
            if (Policy.OnProcessRequestExceptionLog)
                Collector?.LogException($"{FriendlyName}/ProcessRequest unhandled exception: {rq.Message.ToKey()}",ex);

            switch (Policy.OnProcessRequestException)
            {
                case ProcessRequestExceptionBehaviour.DoNothing:
                    break;
                case ProcessRequestExceptionBehaviour.SignalSuccessAndSend500ErrorResponse:
                    rq.SignalSuccess();
                    var rs = rq.ToResponse();
                    rs.Message.Status = "500";
                    rs.Message.StatusDescription = ex.Message;
                    responses.Add(rs);
                    break;
                case ProcessRequestExceptionBehaviour.SignalFailAndDoNothing:
                    rq.SignalFail();
                    break;
                default:
                    throw ex;
            }

            return Task.FromResult(0);
        } 
        #endregion
    }
}