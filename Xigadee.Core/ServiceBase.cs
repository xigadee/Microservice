#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used for components that need a service base.
    /// </summary>
    public abstract class ServiceBase<S> : StatisticsBase<S>, IService, IDisposable
        where S : StatusBase, new()
    {
        #region Events
        /// <summary>
        /// This event can be used to subscribe to status changes.
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged; 
        #endregion
        #region Declarations
		private bool mDisposed = false;

        private ServiceStatus mStatus;
	    #endregion        
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        protected ServiceBase(string name = null):base(name)
        {
            Status = ServiceStatus.Created;
        } 
        #endregion
        #region Finalizer
        /// <summary>
        /// This is the default destructor.
        /// </summary>
        ~ServiceBase()
        {
            Dispose(false);
        } 
        #endregion
        #region Dispose()
        /// <summary>
        /// This method is called when the logger is disposed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Dispose(bool disposing)
        /// <summary>
        /// This method disposes of any specific resources or threads.
        /// </summary>
        /// <param name="disposing">Set to true if disposing, false if called from the Finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            mDisposed = true;
        } 
        #endregion
        #region DisposedCheck(bool throwException = true)
        /// <summary>
        /// This helper method is used to check whether the service is disposed.
        /// </summary>
        /// <param name="throwException">A default property that will throw an ObjectDisposedException</param>
        /// <returns>Returns the disposed status.</returns>
        protected bool DisposedCheck(bool throwException = true)
        {
            if (mDisposed && throwException)
                throw new ObjectDisposedException(GetType().Name);

            return mDisposed;
        }
        #endregion

        #region Status
        /// <summary>
        /// This is the current status of the service.
        /// </summary>
        public ServiceStatus Status 
        {
            get
            {
                return mStatus;
            }
            protected set
            {
                var oldStatus = mStatus;
                mStatus = value;
                if (oldStatus != mStatus)
                    OnStatusChanged(oldStatus, mStatus);
            }
        }



        /// <summary>
        /// This method signals to any event subscribers that the status has changed.
        /// </summary>
        /// <param name="oldStatus">The old status</param>
        /// <param name="newStatus">The new status</param>
        protected virtual void OnStatusChanged(ServiceStatus oldStatus, ServiceStatus newStatus)
        {
            if (StatusChanged != null)
                try
                {
                    StatusChanged(this, new StatusChangedEventArgs() { StatusNew = newStatus, StatusOld = oldStatus });
                }
                catch
                {
                }
        }
        #endregion
        #region Start()
        /// <summary>
        /// This method starts the service.
        /// </summary>
        public void Start()
        {
            if (Status == ServiceStatus.Created)
            {
                Status = ServiceStatus.Starting;
                StartInternal();
            }

            Status = ServiceStatus.Running;
        } 
        #endregion

        #region StartInternal();
        /// <summary>
        /// This abstract method will actually index the service,
        /// </summary>
        protected abstract void StartInternal(); 
        #endregion

        #region Stop()
        /// <summary>
        /// This method stops a running service.
        /// </summary>
        public void Stop()
        {
            if (Status != ServiceStatus.Running)
                return;

            Status = ServiceStatus.Stopping;

            StopInternal();

            Status = ServiceStatus.Stopped;
        } 
        #endregion
        #region StopInternal()
        /// <summary>
        /// This abstract method stops the actual service. You should override this method for your own logic.
        /// </summary>
        protected abstract void StopInternal(); 
        #endregion

        #region ServiceStart(object service, Dictionary<string, string> args)
        /// <summary>
        /// This method starts a specific IServiceProcess service.
        /// </summary>
        /// <param name="service">The service.</param>
        protected virtual void ServiceStart(object service)
        {
            try
            {
                if ((service as IService) != null)
                    ((IService)service).Start();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error starting service [{0}]: {1}", service.GetType().Name, ex.ToString());
                throw;
            }
        } 
        #endregion
        #region ServiceStop(object service)
        /// <summary>
        /// This method stops a specific IServiceProcess service.
        /// </summary>
        /// <param name="service">The service to stop.</param>
        protected virtual void ServiceStop(object service)
        {
            try
            {
                if ((service as IService) != null)
                    ((IService)service).Stop();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error stopping service [{0}]: {1}", service.GetType().Name, ex.ToString());
            }
        } 
        #endregion

        #region ValidateServiceStarted()
        /// <summary>
        /// This method checks whether the system is running.
        /// </summary>
        protected virtual void ValidateServiceStarted()
        {
            //TODO: check whether the system is up and running.
            if (this.Status != ServiceStatus.Running)
                throw new ServiceNotStartedException();
        }
        #endregion
        #region ValidateServiceNotStarted()
        /// <summary>
        /// This method checks whether the system is running.
        /// </summary>
        protected virtual void ValidateServiceNotStarted()
        {
            //TODO: check whether the system is up and running.
            switch (Status)
            {
                case ServiceStatus.Created:
                case ServiceStatus.Stopped:
                    return;
                default:
                    throw new ServiceAlreadyStartedException();
            }
        }
        #endregion

        #region StatisticsRecalculate(S statistics)
        /// <summary>
        /// This method restores the default service behaviour.
        /// </summary>
        protected override void StatisticsRecalculate(S statistics)
        {
        } 
        #endregion
    }
}
