using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xigadee;

namespace Xigadee
{
    public interface IXigadeeMicroservice: IHostedService, IDisposable
    {

    }

    /// <summary>
    /// This class host the Microservice within the AspNetCore application.
    /// </summary>
    /// <seealso cref = "Microsoft.Extensions.Hosting.IHostedService" />
    /// < seealso cref= "https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/multi-container-microservice-net-applications/background-tasks-with-ihostedservice" />
    public class XigadeeService: IXigadeeMicroservice
    {
        public XigadeeService(string name = null
            , string serviceId = null
            , string description = null)
        {
            Service = new Microservice(name, serviceId, description);
        }

        public IMicroservice Service { get; private set; }

        private readonly ILogger _logger;

        private Task _executingTask;

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();



        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogDebug($"GracePeriodManagerService is starting.");
            stoppingToken.Register(() =>
                    LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                LogDebug($"GracePeriod task doing background work.");

                // This eShopOnContainers method is querying a database table 
                // and publishing events into the Event Bus (RabbitMS / ServiceBus)
                //CheckConfirmedGracePeriodOrders();

                //await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }

            LogDebug($"GracePeriod background task is stopping.");

        }

        private void LogDebug(string eventData)
        {

        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it, 
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,cancellationToken));
            }

        }

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}
