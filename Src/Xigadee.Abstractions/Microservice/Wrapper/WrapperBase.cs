using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base wrapper class.
    /// </summary>
    public abstract class WrapperBase
    {
        private Func<ServiceStatus> Status;
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="getStatus">The function used to access the status.</param>
        public WrapperBase(Func<ServiceStatus> getStatus)
        {
            Status = getStatus;
        }

        #region ValidateServiceStarted()
        /// <summary>
        /// This method checks whether the system is running.
        /// </summary>
        protected virtual void ValidateServiceStarted()
        {
            //TODO: check whether the system is up and running.
            if (Status() != ServiceStatus.Running)
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
            switch (Status())
            {
                case ServiceStatus.Created:
                case ServiceStatus.Stopped:
                    return;
                default:
                    throw new ServiceAlreadyStartedException();
            }
        }
        #endregion

    }
}
