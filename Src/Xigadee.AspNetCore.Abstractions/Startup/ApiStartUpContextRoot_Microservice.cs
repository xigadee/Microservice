using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    //Microservice
    public abstract partial class ApiStartUpContextRoot<HE>
    {
        #region MicroserviceConfigure(IServiceCollection services)
        /// <summary>
        /// Configures the singletons.
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void MicroserviceConfigure(IServiceCollection services)
        {
            if (!UseMicroservice)
                return;

            MicroserviceCreate();

            MicroserviceIdentitySet();

            MicroserviceDataCollectionConnect();

            MicroserviceConfigure();

            MicroserviceStatisticsConfigure();

            MicroserviceHostedServiceCreate(services);
        } 
        #endregion

        #region B3a. MicroserviceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceCreate()
        {
            MicroservicePipeline = new MicroservicePipeline();
        }
        /// <summary>
        /// This method sets the Microservice identity in the Host container.
        /// This is needed are pipeline create overrides do not set this.
        /// </summary>
        protected virtual void MicroserviceIdentitySet()
        {
            Host.MicroserviceId = MicroservicePipeline?.Service?.Id;
        }
        #endregion
        #region B3b. MicroserviceDataCollectionConnect()
        /// <summary>
        /// This method connects the DataCollection to the Logger Provider.
        /// </summary>
        protected virtual void MicroserviceDataCollectionConnect()
        {
            //This method pipes the incoming data collection events to the ASP.NET Core logger.
            MicroservicePipeline.OnDataCollection(MicroserviceOnDataCollection);
        }
        /// <summary>
        /// This method is called when a collection event is raised withing the Microservice.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="ev">The event.</param>
        protected virtual void MicroserviceOnDataCollection(OnDataCollectionContext ctx, EventHolder ev)
        {
            switch (ev.DataType)
            {
                //Let's send the most important events to their respective methods.
                case DataCollectionSupport.Statistics:
                    MicroserviceOnDataCollection_Statistics(ctx, ev);
                    break;
                case DataCollectionSupport.Telemetry:
                    MicroserviceOnDataCollection_Telemetry(ctx, ev);
                    break;
                case DataCollectionSupport.EventSource:
                    MicroserviceOnDataCollection_EventSource(ctx, ev);
                    break;
                case DataCollectionSupport.Logger:
                    if (MicroserviceOnDataCollection_Logger(ctx, ev, out LogEventApplication lev))
                        LoggerProvider.AddLogEventToQueue(lev);
                    break;
                default:
                    MicroserviceOnDataCollection_Other(ctx, ev);
                    break;
            }
        }

        /// <summary>
        /// This method is called for the less common Microservice event types.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        protected virtual void MicroserviceOnDataCollection_Other(OnDataCollectionContext ctx, EventHolder ev) { }

        /// <summary>
        /// This method is called when the Microservice logs new statistics.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        protected virtual void MicroserviceOnDataCollection_Statistics(OnDataCollectionContext ctx, EventHolder ev) { }

        /// <summary>
        /// This method is called when the Microservice logs new telemetry.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        protected virtual void MicroserviceOnDataCollection_Telemetry(OnDataCollectionContext ctx, EventHolder ev) { }

        /// <summary>
        /// This method is called when the Microservice logs new event source messages.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        protected virtual void MicroserviceOnDataCollection_EventSource(OnDataCollectionContext ctx, EventHolder ev) { }

        /// <summary>
        /// This method pipes the incoming Microservice logging event in to the ASP.NET Core logging system.
        /// You can override this method to filter out specific messages.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        /// <param name="levOut">The outgoing event.</param>
        /// <returns>Returns true if the event should be logged.</returns>
        protected virtual bool MicroserviceOnDataCollection_Logger(OnDataCollectionContext ctx, EventHolder ev
            , out LogEventApplication levOut)
        {
            levOut = null;
            var le = ev.Data as LogEvent;
            //Check for the unexpected.
            if (le == null)
                return false;

            var lev = new LogEventApplication();
            lev.Message = le.Message;
            lev.Exception = le.Ex;
            lev.Name = ctx.OriginatorId.Name;
            lev.State = le;
            if (le.AdditionalData != null && le.AdditionalData.Count > 0)
            {
                if (lev.FormattedParameters == null)
                    lev.FormattedParameters = new Dictionary<string, object>();
                le.AdditionalData.ForEach(kv => lev.FormattedParameters.Add(kv.Key, kv.Value));
            }

            switch (le.Level)
            {
                case LoggingLevel.Fatal:
                    lev.LogLevel = LogLevel.Critical;
                    break;
                case LoggingLevel.Error:
                    lev.LogLevel = LogLevel.Error;
                    break;
                case LoggingLevel.Warning:
                    lev.LogLevel = LogLevel.Warning;
                    break;
                case LoggingLevel.Info:
                    lev.LogLevel = LogLevel.Information;
                    break;
                case LoggingLevel.Trace:
                    lev.LogLevel = LogLevel.Trace;
                    break;
                case LoggingLevel.Status:
                    lev.LogLevel = LogLevel.Information;
                    break;
                default:
                    lev.LogLevel = LogLevel.None;
                    break;
            }

            levOut = lev;
            return true;
        }
        #endregion
        #region B3c. MicroserviceConfigure()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceConfigure()
        {
            MicroservicePipeline.AdjustPolicyTaskManagerForDebug();

            MicroservicePipeline.AdjustCommunicationPolicyForSingleListenerClient();

            
        }
        #endregion
        #region B3d. MicroserviceStatisticsConfigure()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceStatisticsConfigure()
        {
            MicroservicePipeline.Service.Events.StatisticsIssued += (object sender, StatisticsEventArgs e) => Host?.StatisticsHolder?.Load(e?.Statistics);
        }
        #endregion
        #region B3e. MicroserviceHostedServiceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceHostedServiceCreate(IServiceCollection services)
        {
            MicroserviceHostedService = new MicroserviceHostedService(MicroservicePipeline);
            services.AddSingleton<IHostedService>(MicroserviceHostedService);
        }
        #endregion
    }
}
