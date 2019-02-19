using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// These extension methods simplify the logging of complex data to a consistent framework.
    /// </summary>
    public static partial class DataCollectionExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="secEvent">The security event.</param>
        /// <returns></returns>
        public static async Task SecurityEvent(this IDataCollection collector, SecurityEvent secEvent)
        {
            collector.Write(secEvent, DataCollectionSupport.Security);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="direction">The security event type.</param>
        /// <param name="ex">The exception.</param>
        /// <returns></returns>
        public static async Task SecurityEvent(this IDataCollection collector, SecurityEventDirection direction, Exception ex)
        {
            collector.Write(new SecurityEvent() { Direction = direction, Ex = ex }, DataCollectionSupport.Security);
        }
    }

    /// <summary>
    /// This is the security event direction.
    /// </summary>
    public enum SecurityEventDirection
    {
        /// <summary>
        /// The direction is not set.
        /// </summary>
        NotSet,
        /// <summary>
        /// The event is an incoming verification.
        /// </summary>
        Verification,
        /// <summary>
        /// The event is an outgoing payload signing event.
        /// </summary>
        Signing
    }
}
