using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to wrap the dispatch functionality.
    /// </summary>
    public class DispatchWrapper: WrapperBase, IMicroserviceDispatch
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchWrapper"/> class.
        /// </summary>
        /// <param name="serviceHandlers">The service handler collection.</param>
        /// <param name="executeOrEnqueue">The execute or enqueue action.</param>
        /// <param name="getStatus">The get status function.</param>
        /// <param name="traceEnabled">if set to <c>true</c> trace is enabled for payloads.</param>
        protected internal DispatchWrapper(IServiceHandlers serviceHandlers, Action<TransmissionPayload, string> executeOrEnqueue, Func<ServiceStatus> getStatus, bool traceEnabled)
            : base(getStatus)
        {
            Name = GetType().Name;
            ExecuteOrEnqueue = executeOrEnqueue;
            ServiceHandlers = serviceHandlers;
            PayloadTraceEnabled = traceEnabled;
        }
        #endregion

        /// <summary>
        /// Identifies whether the payload trace is enabled.
        /// </summary>
        public bool PayloadTraceEnabled { get; }
        /// <summary>
        /// Gets the serialization container.
        /// </summary>
        public IServiceHandlers ServiceHandlers { get; }
        /// <summary>
        /// Gets the execute or enqueue action.
        /// </summary>
        protected Action<TransmissionPayload, string> ExecuteOrEnqueue { get; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        protected string Name { get; }

        /// <summary>
        /// This method injects a payload in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="payload">The transmission payload to execute.</param>
        public virtual void Process(TransmissionPayload payload)
        {
            ValidateServiceStarted();

            if (PayloadTraceEnabled)
            {
                payload.TraceEnabled = true;
                payload.TraceWrite($"{Name} received.");
            }

            ExecuteOrEnqueue(payload, $"{Name} method request");
        }
    }
}
