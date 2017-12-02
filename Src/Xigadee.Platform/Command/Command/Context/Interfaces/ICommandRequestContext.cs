using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface extends the Command Context and adds the specific request data.
    /// </summary>
    /// <seealso cref="Xigadee.ICommandContext" />
    public interface ICommandRequestContext: ICommandContext
    {
        /// <summary>
        /// Gets the correlation identifier.
        /// </summary>
        string CorrelationId { get; }
        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// Gets the service message.
        /// </summary>
        ServiceMessage Message { get; }
        /// <summary>
        /// Gets the request.
        /// </summary>
        TransmissionPayload Request { get; }
        /// <summary>
        /// Gets the response collection.
        /// </summary>
        List<TransmissionPayload> Responses { get; }
    }
}