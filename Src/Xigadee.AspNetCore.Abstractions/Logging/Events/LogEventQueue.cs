using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// Queue for log events.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LogEventQueue: ILogEventQueue
    {
        #region Declarations
        private readonly ConcurrentQueue<LogEventApplication> _eventQueue = new ConcurrentQueue<LogEventApplication>();
        private readonly ILogEventPublisher _logEventPublisher;

        private readonly ManualResetEventSlim _mrseQueueReader;
        private readonly Thread _threadQueueReader;

        private bool _queueReaderActive;
        private readonly int _loopPauseTimeInMs;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventQueue"/> class.
        /// </summary>
        /// <param name="logEventPublisher">The log event publisher.</param>
        /// <param name="loopPauseTimeInMs">This is the loop pause time in milliseconds. The default is to cycle every second.</param>
        /// <exception cref="ArgumentNullException">configuration - LogEventQueue</exception>
        public LogEventQueue(ILogEventPublisher logEventPublisher, int loopPauseTimeInMs = 1000)
        {
            _loopPauseTimeInMs = loopPauseTimeInMs;
            _logEventPublisher = logEventPublisher;

            _threadQueueReader = new Thread(new ParameterizedThreadStart(Start));

            _mrseQueueReader = new ManualResetEventSlim(false);

            _threadQueueReader.Start();
        }
        #endregion
        #region Dispose()
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _queueReaderActive = false;

            UnlockQueueReader();

            _threadQueueReader.Join();
            //Pass on the good news.
            _logEventPublisher.Dispose();
        }
        #endregion

        #region Hold/Release()
        /// <summary>
        /// If this is set to true, the queue will hold the incoming log messages internally.
        /// </summary>
        private bool OnHold { get; set; }

        /// <summary>
        /// This method makes the queue hold incoming messages in memory until the Release() method is called.
        /// </summary>
        public void Hold() => OnHold = true;

        /// <summary>
        /// This releases the queue and sets it to poll immediately.
        /// </summary>
        public void Release()
        {
            OnHold = false;

            UnlockQueueReader();
        }
        #endregion
        #region Add(LogEventApplication logEvent)
        /// <summary>
        /// Adds the specified log event to the queue.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        public void Add(LogEventApplication logEvent)
        {
            _eventQueue.Enqueue(logEvent);

            UnlockQueueReader();
        } 
        #endregion


        private void PublishLogEvent(LogLevel level, string message, string header, Exception ex = null)
        {
            var logEvent = new LogEventApplication { LogLevel = level, Exception = ex, Name = header, Message = message };

            PublishLogEvent(logEvent);
        }

        private void PublishLogEvent(LogEventApplication logEvent) => _logEventPublisher.Publish(logEvent);

        private void UnlockQueueReader() => _mrseQueueReader.Set();

        private void Start(object state)
        {
            _queueReaderActive = true;

            try
            {
                // Loop to infinity until an exception is called for the thread or _queueReaderActive is set to false.
                while (_queueReaderActive)
                {
                    _mrseQueueReader.Reset();

                    if (!OnHold)
                        while (_eventQueue.TryDequeue(out var logEvent))
                            PublishLogEvent(logEvent);

                    //We wait for a short while, and then check anyway.
                    //We don't want this to keep polling, so we set a delay. 
                    //However, any incoming messages will trigger it to loop.
                    _mrseQueueReader.Wait(_loopPauseTimeInMs);
                }
            }
            catch (ThreadAbortException)
            {
                Stop();

                PublishLogEvent(LogLevel.Information, "Shutting down", "Logging");
            }
            catch (Exception ex)
            {
                Stop();

                PublishLogEvent(LogLevel.Critical, "Unhandled exception shutting down", "Logging", ex);
            }
        }

        private void Stop()
        {
            _queueReaderActive = false;
        }
    }
}