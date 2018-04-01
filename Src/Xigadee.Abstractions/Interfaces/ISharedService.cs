using System;

namespace Xigadee
{
    /// <summary>
    /// This interface is used by the Microservice to share singleton services between components.
    /// </summary>
    public interface ISharedService
    {
        /// <summary>
        /// Registers the service.
        /// </summary>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>Returns true if registered successfully./returns>
        bool RegisterService<I>(I instance, string serviceName = null) where I : class;
        /// <summary>
        /// Registers the service.
        /// </summary>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>Returns true if registered successfully./returns>
        bool RegisterService<I>(Lazy<I> instance, string serviceName = null) where I : class;
        /// <summary>
        /// Removes the service.
        /// </summary>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <returns>Returns true if removed successfully./returns>
        bool RemoveService<I>() where I : class;
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <returns>The interface implementation</returns>
        I GetService<I>() where I : class;
        /// <summary>
        /// Determines whether this instance has a service registered.
        /// </summary>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <returns>
        ///   <c>true</c> if this collection has the service; otherwise, <c>false</c>.
        /// </returns>
        bool HasService<I>() where I : class;
    }
}
