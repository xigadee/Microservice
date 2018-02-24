using System;
using System.Collections.Generic;
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
            , string description = null
            , IEnumerable<PolicyBase> policy = null
            , IEnumerable<Tuple<string, string>> properties = null
            , IEnvironmentConfiguration config = null
            , Action<IMicroservice> assign = null
            , Action<IEnvironmentConfiguration> configAssign = null
            , bool addDefaultJsonPayloadSerializer = true
            , bool addDefaultPayloadCompressors = true
            , string serviceVersionId = null
            , Type serviceReference = null)
        {
            Pipeline = new MicroservicePipeline(name, serviceId, description, policy
                , properties, config, assign
                , configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors
                , serviceVersionId, serviceReference);
        }

        public MicroservicePipeline Pipeline { get; private set; }

        private Task _executingTask;


        private void LogDebug(string eventData)
        {

        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            //_executingTask = ExecuteAsync(_stoppingCts.Token);
            TryStart();
            //// If the task is completed then return it, 
            //// this will bubble cancellation and failure to the caller
            //if (_executingTask.IsCompleted)
            //{
            //    return _executingTask;
            //}

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
                TryStop(false);
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,cancellationToken));
            }

        }

        public virtual void Dispose()
        {
            TryStop(true);
        }

        public bool TryStart()
        {
            if (Pipeline.Service.Status != ServiceStatus.Created)
                return false;

            Pipeline.Service.Start();
            return true;
        }

        public bool TryStop(bool force)
        {
            if (Pipeline.Service.Status != ServiceStatus.Running)
                return false;

            Pipeline.Service.Stop();

            return true;
        }
    }
}
