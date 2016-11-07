using System.Collections.Specialized;
using System.Runtime.Remoting.Messaging;

namespace Xigadee
{
    /// <summary>
    /// Helper to get and set Command Context Data
    /// </summary>
    public static class CommandContext
    {
        private const string CorrelationKeyName = "Xigadee.CommandContext.CorrelationKey";

        /// <summary>
        /// Correlation Key
        /// </summary>
        public static string CorrelationKey
        {
            get { return CallContext.LogicalGetData(CorrelationKeyName) as string; }
            set
            {
                if (value == null)
                    CallContext.FreeNamedDataSlot(CorrelationKeyName);
                else
                    CallContext.LogicalSetData(CorrelationKeyName, value);  
            }
        }
    }
}
