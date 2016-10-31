using System.Runtime.Remoting.Messaging;

namespace Xigadee
{
    /// <summary>
    /// Helper to get and set Command Context Data
    /// </summary>
    public static class CommandContext
    {
        /// <summary>
        /// Correlation Key
        /// </summary>
        public static string CorrelationKey
        {
            get { return CallContext.LogicalGetData(nameof(CorrelationKey)) as string; }
            set { CallContext.LogicalSetData(nameof(CorrelationKey), value); }
        }
    }
}
