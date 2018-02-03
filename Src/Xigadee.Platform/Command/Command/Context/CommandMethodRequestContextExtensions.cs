using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This static class contains a number of extension methods to make using an inline command simpler.
    /// </summary>
    public static class CommandMethodRequestContextExtensions
    {
        /// <summary>
        /// This extension method is used to set the response for an inline message.
        /// </summary>
        /// <typeparam name="C">The entity type.</typeparam>
        /// <param name="c">The incoming parameters.</param>
        /// <param name="status">The response status.</param>
        /// <param name="response">The optional response object.</param>
        /// <param name="description">The optional response description</param>
        public static void ResponseSet<C>(this ICommandRequestContext c, int status, C response = default(C), string description = null)
        {
            TransmissionPayload ars = c.Request.ToResponse();

            ars.Message.StatusSet(status, description);

            if (!response.Equals(default(C)))
                ars.Message.Holder = c.ServiceHandlers.PayloadSerialize(response);

            c.Responses.Add(ars);
        }

        /// <summary>
        /// This extension method is used to set the response for an inline message.
        /// </summary>
        /// <param name="c">The incoming context.</param>
        /// <param name="status">The response status.</param>
        /// <param name="description">The optional response description</param>
        public static void ResponseSet(this ICommandRequestContext c, int status, string description = null)
        {
            TransmissionPayload ars = c.Request.ToResponse();

            ars.Message.StatusSet(status, description);

            c.Responses.Add(ars);
        }

        /// <summary>
        /// This extension method is used to try and get the payload object from the incoming payload.
        /// </summary>
        /// <typeparam name="C">The payload entity type.</typeparam>
        /// <param name="c">The context.</param>
        /// <param name="response">The outgoing payload object.</param>
        /// <returns>Returns true if the payload is present.</returns>
        public static bool RequestPayloadTryGet<C>(this ICommandRequestContext c, out C response)
        {
            response = default(C);

            return c.PayloadSerializer.PayloadTryDeserialize<C>(c.Request, out response);
        }

        /// <summary>
        /// This extension method extracts and deserializes the payload object from the message binary blob.
        /// </summary>
        /// <typeparam name="C">The payload entity type.</typeparam>
        /// <param name="c">The context.</param>
        /// <returns>Returns the payload object.</returns>
        public static C RequestPayloadGet<C>(this ICommandRequestContext c)
        {
            return c.PayloadSerializer.PayloadDeserialize<C>(c.Request);
        }
    }
}
