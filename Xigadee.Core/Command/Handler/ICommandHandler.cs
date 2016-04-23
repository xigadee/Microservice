using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ICommandHandler
    {
        void Initialise(MessageFilterWrapper key, Func<TransmissionPayload, List<TransmissionPayload>, Task> action);

        MessageFilterWrapper Key { get; }

        Func<TransmissionPayload, List<TransmissionPayload>, Task> Action { get; }

        Task Execute(TransmissionPayload rq, List<TransmissionPayload> rs);

        ICommandHandlerStatistics HandlerStatistics { get; }
    }
}