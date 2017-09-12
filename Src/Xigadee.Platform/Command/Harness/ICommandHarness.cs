using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is the outwards facing interface for the command harness.
    /// </summary>
    public interface ICommandHarness
    {
        /// <summary>
        /// This is the dependencies class. This class can be shared with another harness.
        /// </summary>
        CommandHarnessDependencies Dependencies { get; }
        /// <summary>
        /// Occurs when a CommandHarnessRequest object is created.
        /// </summary>
        event EventHandler<CommandHarnessEventArgs> OnEvent;
        /// <summary>
        /// Occurs when a request CommandHarnessRequest object is created.
        /// </summary>
        event EventHandler<CommandHarnessEventArgs> OnEventRequest;
        /// <summary>
        /// Occurs when a response CommandHarnessRequest object is created.
        /// </summary>
        event EventHandler<CommandHarnessEventArgs> OnEventResponse;
        /// <summary>
        /// Occurs when an outgoing CommandHarnessRequest object is created.
        /// </summary>
        event EventHandler<CommandHarnessEventArgs> OnEventOutgoing;

        /// <summary>
        /// This is the collection of the traffic in and out of the command harness
        /// </summary>
        ConcurrentDictionary<long, CommandHarnessTraffic> Traffic { get; }

        /// <summary>
        /// Gets the dispatcher, which can be used to send requests to the command.
        /// </summary>
        IMicroserviceDispatch Dispatcher { get; }

        /// <summary>
        /// Gets the registered command methods.
        /// </summary>
        Dictionary<MessageFilterWrapper, bool> RegisteredCommandMethods { get; }
        /// <summary>
        /// Gets the registered schedules.
        /// </summary>
        Dictionary<CommandJobSchedule, bool> RegisteredSchedules { get; }
        /// <summary>
        /// Executes the schedule with the unique identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns true if the schedule was resolved and executed.</returns>
        bool ScheduleExecute(Guid id);
        /// <summary>
        /// Executes the schedule with the name specified.
        /// </summary>
        /// <param name="name">The name of the schedule.</param>
        /// <returns>Returns true if the schedule was resolved and executed.</returns>
        bool ScheduleExecute(string name);
        /// <summary>
        /// Determines whether the collection has the specified schedule.
        /// </summary>
        /// <param name="name">The schedule name.</param>
        /// <returns>Returns true if the schedule exists</returns>
        bool HasSchedule(string name);
        /// <summary>
        /// Determines whether the collection has the specified schedule.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns true if the schedule exists</returns>
        bool HasSchedule(Guid id);
    }
}