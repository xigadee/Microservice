#region using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //TaskManager
    public partial class Microservice
    {
        /// <summary>
        /// This event is raised when the service start begins
        /// </summary>
        public event EventHandler<StartEventArgs> StartRequested;
        /// <summary>
        /// This event is raised when the service start completes.
        /// </summary>
        public event EventHandler<StartEventArgs> StartCompleted;
        /// <summary>
        /// This event is raised when a stop request begins.
        /// </summary>
        public event EventHandler<StopEventArgs> StopRequested;
        /// <summary>
        /// This event is raised when the service has stopped.
        /// </summary>
        public event EventHandler<StopEventArgs> StopCompleted;
        /// <summary>
        /// This event is raised when the service has stopped.
        /// </summary>
        public event EventHandler<MicroserviceStatusEventArgs> ComponentStatusChange;

        /// <summary>
        /// This event will be triggered when the Microservice statistics have been calculated.
        /// </summary>
        public event EventHandler<StatisticsEventArgs> StatisticsIssued;

        /// <summary>
        /// This event will be thrown if an incoming message cannot be resolved against a command.
        /// </summary>
        public event EventHandler<ProcessRequestUnresolvedEventArgs> ProcessRequestUnresolved;
        /// <summary>
        /// This event will be raised when a process request errors.
        /// </summary>
        public event EventHandler<ProcessRequestErrorEventArgs> ProcessRequestError;

        //public event EventHandler<AutotuneEventArgs> AutotuneEvent;


        protected virtual void OnProcessRequestUnresolved(TransmissionPayload payload)
        {
            try
            {
                ProcessRequestUnresolved?.Invoke(this, new ProcessRequestUnresolvedEventArgs() { Payload = payload });
            }
            catch (Exception ex)
            {
                mLogger?.LogException("OnUnhandledRequest / external exception thrown on event", ex);
            }
        }

        protected virtual void OnProcessRequestError(TransmissionPayload payload, Exception pex)
        {
            try
            {
                ProcessRequestError?.Invoke(this, new ProcessRequestErrorEventArgs() { Payload = payload, Ex = pex });
            }
            catch (Exception ex)
            {
                mLogger?.LogException("OnUnhandledRequest / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStartRequested()
        {
            try
            {
                StartRequested?.Invoke(this, new StartEventArgs() { ConfigurationOptions = ConfigurationOptions });
            }
            catch (Exception ex)
            {
                mLogger?.LogException("OnStartRequested / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStartCompleted()
        {
            try
            {
                StartCompleted?.Invoke(this, new StartEventArgs());
            }
            catch (Exception ex)
            {
                mLogger?.LogException("OnStartCompleted / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStopRequested()
        {
            try
            {
                StopRequested?.Invoke(this, new StopEventArgs());
            }
            catch (Exception ex)
            {
                mLogger?.LogException("OnStopRequested / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStopCompleted()
        {
            try
            {
                StopCompleted?.Invoke(this, new StopEventArgs());
            }
            catch (Exception ex)
            {
                mLogger?.LogException("OnStopCompleted / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStatisticsIssued(MicroserviceStatistics statistics)
        {
            try
            {
                StatisticsIssued?.Invoke(this, new StatisticsEventArgs() { Statistics = statistics }); 
            }
            catch (Exception ex)
            {
                mLogger?.LogException("Action_OnStatistics / external exception thrown on event", ex);
            }
        } 
    }

    public class ProcessRequestUnresolvedEventArgs: EventArgs
    {
        public TransmissionPayload Payload { get; set; }
    }

    public class ProcessRequestErrorEventArgs: ProcessRequestUnresolvedEventArgs
    {
        public Exception Ex { get; set; }
    }

    public class StartEventArgs: EventArgs
    {
        public MicroserviceConfigurationOptions ConfigurationOptions { get; set; }
    }

    public class StopEventArgs: EventArgs
    {
    }

    public class StatisticsEventArgs: EventArgs
    {
        public MicroserviceStatistics Statistics { get; set; }
    }

    public class AutotuneEventArgs: EventArgs
    {

    }

}
