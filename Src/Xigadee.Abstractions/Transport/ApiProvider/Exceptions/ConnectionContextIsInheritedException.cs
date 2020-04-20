using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is throw when a child connector attempts to reset the settings and they are inherited from 
    /// the parent connector. You should change the settings on the parent instead and they will be inherited.
    /// </summary>
    public class ConnectionContextIsInheritedException : Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ConnectionContextIsInheritedException()
        {
        }
    }
}
