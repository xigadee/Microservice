using System;
using System.Diagnostics;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This context is used to hold the necessary data for an inline command request.
    /// </summary>
    public class CommandScheduleInlineContext: CommandContextBase<ICommandOutgoing>
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="schedule">The incoming schedule.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="serializer">The serialization container.</param>
        /// <param name="collector">The data collector.</param>
        /// <param name="sharedServices">The shared service context.</param>
        /// <param name="originatorId">This is the Microservice identifiers.</param>
        /// <param name="outgoingRequest">This is the outgoing request initiator.</param>
        public CommandScheduleInlineContext(Schedule schedule, CancellationToken token
            , IPayloadSerializationContainer serializer
            , IDataCollection collector
            , ISharedService sharedServices
            , MicroserviceId originatorId
            , ICommandOutgoing outgoingRequest) :base(serializer, collector, sharedServices, originatorId, outgoingRequest)
        {
            Schedule = schedule;
            Token = token;
        }
        /// <summary>
        /// The incoming request.
        /// </summary>
        public Schedule Schedule { get; }
        /// <summary>
        /// The outgoing responses.
        /// </summary>
        public CancellationToken Token { get; }

        /// <summary>
        /// This is the request Id.
        /// </summary>
        public Guid Id => Schedule.Id;
    }
}
