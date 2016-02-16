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
    public partial class MicroserviceBase
    {
        public event EventHandler<StartEventArgs> StartRequested;
        public event EventHandler<StartEventArgs> StartCompleted;

        public event EventHandler<StopEventArgs> StopRequested;
        public event EventHandler<StopEventArgs> StopCompleted;

        public event EventHandler<StatisticsEventArgs> StatisticsIssued;

        public event EventHandler<ProcessRequestUnresolvedEventArgs> ProcessRequestUnresolved;

        public event EventHandler<ProcessRequestErrorEventArgs> ProcessRequestError;

        //public event EventHandler<AutotuneEventArgs> AutotuneEvent;


        protected virtual void OnProcessRequestUnresolved(TransmissionPayload payload)
        {
            try
            {
                if (ProcessRequestUnresolved != null)
                    ProcessRequestUnresolved(this, new ProcessRequestUnresolvedEventArgs() { Payload = payload });
            }
            catch (Exception ex)
            {
                mLogger.LogException("OnUnhandledRequest / external exception thrown on event", ex);
            }
        }

        protected virtual void OnProcessRequestError(TransmissionPayload payload, Exception pex)
        {
            try
            {
                if (ProcessRequestError != null)
                    ProcessRequestError(this, new ProcessRequestErrorEventArgs() { Payload = payload, Ex = pex });
            }
            catch (Exception ex)
            {
                mLogger.LogException("OnUnhandledRequest / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStartRequested()
        {
            try
            {
                if (StartRequested != null)
                    StartRequested(this, new StartEventArgs() { ConfigurationOptions = ConfigurationOptions });
            }
            catch (Exception ex)
            {
                mLogger.LogException("OnStartRequested / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStartCompleted()
        {
            try
            {
                if (StartCompleted != null)
                    StartCompleted(this, new StartEventArgs());
            }
            catch (Exception ex)
            {
                mLogger.LogException("OnStartCompleted / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStopRequested()
        {
            try
            {
                if (StopRequested != null)
                    StopRequested(this, new StopEventArgs());
            }
            catch (Exception ex)
            {
                mLogger.LogException("OnStopRequested / external exception thrown on event", ex);
            }
        }

        protected virtual void OnStopCompleted()
        {
            try
            {
                if (StopCompleted != null)
                    StopCompleted(this, new StopEventArgs());
            }
            catch (Exception ex)
            {
                mLogger.LogException("OnStopCompleted / external exception thrown on event", ex);
            }
        }


        protected virtual void OnStatisticsIssued(MicroserviceStatistics statistics)
        {
            try
            {
                if (StatisticsIssued != null)
                    StatisticsIssued(this, new StatisticsEventArgs() { Statistics = statistics }); 
            }
            catch (Exception ex)
            {
                mLogger.LogException("Action_OnStatistics / external exception thrown on event", ex);
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
