using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to specify a module class that requires start stop support.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ModuleStartStopAttribute : Attribute
    {
        /// <summary>
        /// This is the constructor.
        /// </summary>
        public ModuleStartStopAttribute(ModuleStartStopMode mode = ModuleStartStopMode.StartStop)
        {
            Mode = mode;
        }

        /// <summary>
        /// This property specifies that the container should autoconnect the module.
        /// </summary>
        public ModuleStartStopMode Mode { get; }
    }

    /// <summary>
    /// THis enumeration is used to specify the start stop mode for the Module.
    /// </summary>
    [Flags]
    public enum ModuleStartStopMode
    {
        /// <summary>
        /// No additional features.
        /// </summary>
        None = 0,
        /// <summary>
        /// Create the module automatically. This requires a parameterless constructor.
        /// </summary>
        Create = 1,
        /// <summary>
        /// This method is called the load the context.
        /// </summary>
        Load = 2,
        /// <summary>
        /// Autoconnect the module to the context before it starts.
        /// </summary>
        Connect = 4,
        /// <summary>
        /// This mode starts the service.
        /// </summary>
        Start = 8,
        /// <summary>
        /// This mode stops the service.
        /// </summary>
        Stop = 16,
        /// <summary>
        /// This is the default behaviour.
        /// </summary>
        StartStop = 24,
        /// <summary>
        /// This option ensures all steps are executed.
        /// </summary>
        All = 31
    }
}
