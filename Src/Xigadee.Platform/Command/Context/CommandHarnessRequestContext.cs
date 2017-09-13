using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This context is used to hold the necessary data for an in-line command request.
    /// </summary>
    public class CommandHarnessRequestContext: CommandRequestContextBase<IMicroserviceDispatch>
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="harness">The command harness</param>
        /// <param name="args">The command event arguments.</param>
        /// <param name="rq">The incoming request.</param>
        public CommandHarnessRequestContext(ICommandHarness harness, CommandHarnessEventArgs args, TransmissionPayload rq) 
            :base(rq
                , new List<TransmissionPayload>()
                , harness.Dependencies.PayloadSerializer
                , harness.Dependencies.Collector
                , harness.Dependencies.SharedService
                , harness.Dependencies.OriginatorId
                , harness.Dispatcher
                 )
        {
            CommandHarness = harness;
            EventArgs = args;
        }
        /// <summary>
        /// Gets the command harness.
        /// </summary>
        public ICommandHarness CommandHarness { get; }
        /// <summary>
        /// Gets the current event arguments.
        /// </summary>
        public CommandHarnessEventArgs EventArgs { get; }
    }
}
