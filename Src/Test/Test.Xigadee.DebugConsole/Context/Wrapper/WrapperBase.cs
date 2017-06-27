using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This wrapper is used to contain a set of services behind a command generic wrapper that allows them to share common menus.
    /// </summary>
    /// <typeparam name="K">The entity key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class WrapperBase<K,E> : IConsolePersistence<K,E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// This event is triggered when the service status changes.
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        /// <summary>
        /// This is the current service status.
        /// </summary>
        public abstract ServiceStatus Status { get; }
        /// <summary>
        /// This is the persistence handler.
        /// </summary>
        public IRepositoryAsync<K, E> Persistence { get;set; }
        /// <summary>
        /// This is the name of the service.
        /// </summary>
        public abstract string Name { get; protected set; }

        /// <summary>
        /// This method starts the service.
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// This method stops the service.
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// This method triggers the event when the status changes for the service.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(sender, e);
        }

    }
}
