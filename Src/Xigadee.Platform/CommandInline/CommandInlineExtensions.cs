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
    public static class CommandInlineExtensions
    {
        /// <summary>
        /// This extension method is used to set the response for an inline message.
        /// </summary>
        /// <typeparam name="C">The entity type.</typeparam>
        /// <param name="c">The incoming parameters.</param>
        /// <param name="status">The response status.</param>
        /// <param name="response">The optional response object.</param>
        /// <param name="description">The optional response description</param>
        public static void ResponseSet<C>(
            this (TransmissionPayload rq, List<TransmissionPayload> rsOut, IPayloadSerializationContainer serializer) c
                , int status, C response = default(C), string description = null)
        {
            TransmissionPayload ars = c.rq.ToResponse();

            ars.Message.StatusSet(status, description);

            if (!response.Equals(default(C)))
                ars.Message.Blob = c.serializer.PayloadSerialize(response);

            c.rsOut.Add(ars);
        }

        /// <summary>
        /// This extension method is used to set the response for an inline message.
        /// </summary>
        /// <typeparam name="C">The entity type.</typeparam>
        /// <param name="c">The incoming parameters.</param>
        /// <param name="status">The response status.</param>
        /// <param name="response">The optional response object.</param>
        /// <param name="description">The optional response description</param>
        public static void ResponseSet(
            this (TransmissionPayload rq, List<TransmissionPayload> rsOut, IPayloadSerializationContainer serializer) c
                , int status, string description = null)
        {
            TransmissionPayload ars = c.rq.ToResponse();

            ars.Message.StatusSet(status, description);

            c.rsOut.Add(ars);
        }

        /// <summary>
        /// This extension method is used to set the response for an inline message.
        /// </summary>
        /// <typeparam name="C">The entity type.</typeparam>
        /// <param name="c">The incoming parameters.</param>
        /// <param name="response">The optional response object.</param>
        /// <returns>Returns true if the item was present.</returns>
        public static bool RequestTryGet<C>(
            this (TransmissionPayload rq, List<TransmissionPayload> rsOut, IPayloadSerializationContainer serializer) c
                , out C response)
        {
            response = default(C);

            return c.serializer.PayloadTryDeserialize<C>(c.rq, out response);
        }
    }
}
