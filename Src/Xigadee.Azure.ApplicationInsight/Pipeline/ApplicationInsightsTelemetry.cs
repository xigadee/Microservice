//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xigadee;
//using Microsoft.ApplicationInsights.Channel;
//using System.Reflection;
//using Microsoft.ApplicationInsights.Extensibility;
//using Microsoft.ApplicationInsights;

//namespace Xigadee
//{

//    public class ApplicationInsightsTelemetry
//    {
//        public TelemetryClient Client { get; private set; }

//        public ApplicationInsightsTelemetry() : this(null, null)
//        {
//        }

//        public ApplicationInsightsTelemetry(string componentName, string telemetryKey)
//        {
//            try
//            {
//                TelemetryConfiguration.Active.InstrumentationKey = telemetryKey;
//                Client = new TelemetryClient();
//                Client.Context.Component.Version =
//                    (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())
//                        .GetName().Version.ToString();

//                Client.Context.Properties["MachineName"] = Environment.MachineName;
//                if (!string.IsNullOrEmpty(componentName))
//                    Client.Context.Properties["ComponentName"] = componentName;

//                TrackEvent($"StartUp:{componentName}", new Dictionary<string, string> { { "AssemblyVersion", Client.Context.Component.Version } });
//            }
//            catch (Exception ex)
//            {
//            }
//        }

//        public void TrackException(Exception exception, IDictionary<string, string> properties = null)
//        {
//            try
//            {
//                if (Client != null)
//                    Client.TrackException(exception, properties);
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public void TrackEvent(string eventName, IDictionary<string, string> properties = null)
//        {
//            try
//            {
//                if (Client != null)
//                    Client.TrackEvent(eventName, properties);
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public void TrackMetric(string metricName, double value)
//        {
//            try
//            {
//                if (Client != null)
//                    Client.TrackMetric(metricName, value);
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public void TrackRequest(string requestName, DateTimeOffset timestamp, TimeSpan duration, string responseCode, bool isSuccess)
//        {
//            try
//            {
//                if (Client != null)
//                    Client.TrackRequest(requestName, timestamp, duration, responseCode, isSuccess);

//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public IDisposable TrackRequest(string requestName)
//        {
//            return new RequestTimer(requestName, this);
//        }

//        public void TrackView(string viewName)
//        {
//            if (Client != null)
//                Client.TrackPageView(viewName);
//        }
//    }

//    internal class RequestTimer: IDisposable
//    {
//        private readonly string _requestName;
//        private readonly ITelemetryService _telemetryService;
//        private readonly Stopwatch _stopwatch;
//        private readonly DateTimeOffset _timestamp;

//        public RequestTimer(string requestName, ITelemetryService telemetryService)
//        {
//            _requestName = requestName;
//            _telemetryService = telemetryService;
//            _timestamp = DateTimeOffset.UtcNow;
//            _stopwatch = Stopwatch.StartNew();
//        }

//        public void Dispose()
//        {
//            _stopwatch.Stop();
//            bool succeeded = Marshal.GetExceptionCode() == 0;
//            _telemetryService.TrackRequest(_requestName, _timestamp, _stopwatch.Elapsed, succeeded ? "200" : "500", succeeded);
//        }
//    }

//    public class AppInsightTelemetry: ITelemetry, ILogger
//    {
//        private readonly LoggingLevel mLoggingLevel;

//        public AppInsightTelemetry(ITelemetryService telemetryService, LoggingLevel loggingLevel)
//        {
//            mLoggingLevel = loggingLevel;
//        }

//        #region ITelemetry
//        public void TrackMetric(string metricName, double value)
//        {
//            mTelemetryService.TrackMetric(metricName, value);
//        }
//        #endregion

//        #region ILogger

//        public Task Log(LogEvent logEvent)
//        {
//            if (logEvent.Level < mLoggingLevel)
//                return Task.CompletedTask;

//            try
//            {
//                Dictionary<string, string> customData = new Dictionary<string, string> { { "LoggingLevel", logEvent.Level.ToString() } };
//                if (logEvent.AdditionalData != null || !string.IsNullOrEmpty(logEvent.Message))
//                {
//                    var data = customData;
//                    logEvent.AdditionalData?.ForEach(kvp => data[kvp.Key] = kvp.Value);
//                    if (!string.IsNullOrEmpty(logEvent.Message))
//                        customData["Message"] = logEvent.Message;

//                    if (!string.IsNullOrEmpty(logEvent.Category))
//                        customData["Category"] = logEvent.Category;
//                }

//                // Don't log non errors that have exceptions as exceptions i.e. warnings / info
//                if (logEvent.Ex != null && logEvent.Level >= LoggingLevel.Error)
//                {
//                    mTelemetryService.TrackException(logEvent.Ex, customData);
//                }
//                else
//                {
//                    if (logEvent.Ex != null)
//                        customData["Exception"] = logEvent.Ex.ToString();

//                    mTelemetryService.TrackEvent(logEvent.Level + (!string.IsNullOrEmpty(logEvent.Category) ? $":{logEvent.Category}" : string.Empty), customData);
//                }
//            }
//            catch (Exception ex)
//            {
//                mTelemetryService.TrackException(ex, new Dictionary<string, string> { { "Message", "Unable to log correctly" } });
//                throw;
//            }

//            return Task.CompletedTask;
//        }

//        #endregion
//    }

//}
