using System;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This wrapper is used to contain the Microservice within the console application.
    /// </summary>
    public class MicroservicePersistenceWrapper<K,E> : WrapperBase<K,E>
        where K: IEquatable<K>
    {
        private Action<MicroservicePersistenceWrapper<K, E>> mConfigure;
        private Action<MicroservicePersistenceWrapper<K, E>> mInit;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="name">The service name.</param>
        /// <param name="context">This is config context.</param>
        /// <param name="configure">This is the link to configuration pipeline for the Microservice</param>
        /// <param name="init"></param>
        public MicroservicePersistenceWrapper(string name
            , ConsoleSettings context
            , Action<MicroservicePersistenceWrapper<K,E>> configure
            , Action<MicroservicePersistenceWrapper<K,E>> init = null)
        {
            if (configure == null)
                throw new ArgumentNullException("configure");
            if (context == null)
                throw new ArgumentNullException("context");

            Context = context;
            Name = name;
            mConfigure = configure;
            mInit = init;

            PipelineInitialise();
        }
        /// <summary>
        /// This is the context that holds the console settings.
        /// </summary>
        public ConsoleSettings Context { get; }
        /// <summary>
        /// The current Microservice status.
        /// </summary>
        public override ServiceStatus Status
        {
            get { return Pipeline?.Service?.Status ?? ServiceStatus.Created; }
        }
        /// <summary>
        /// The service name.
        /// </summary>
        public override string Name { get; protected set; }
        /// <summary>
        /// This is the data collector.
        /// </summary>
        public DebugMemoryDataCollector Collector { get; set; }
        /// <summary>
        /// This is the pipeline used to configure the Microservice.
        /// </summary>
        public MicroservicePipeline Pipeline { get; protected set; }
        /// <summary>
        /// This is the Microservice configuration, or null if the pipeline is not set.
        /// </summary>
        public IEnvironmentConfiguration Config { get { return Pipeline?.ToConfiguration(); } }
        /// <summary>
        /// This method starts the service.
        /// </summary>
        public override void Start()
        {
            mConfigure?.Invoke(this);

            Pipeline.Service.StatusChanged += OnStatusChanged;
            Pipeline.Service.Events.ProcessRequestError += Events_ProcessRequestError;
            Pipeline.Service.Events.ProcessRequestUnresolved += Events_ProcessRequestUnresolved;

            Pipeline.Start();
        }

        private void Events_ProcessRequestUnresolved(object sender, DispatcherRequestUnresolvedEventArgs e)
        {
        }

        private void Events_ProcessRequestError(object sender, ProcessRequestErrorEventArgs e)
        {
        }


        private void PipelineInitialise()
        {
            Pipeline = new MicroservicePipeline(Name);
            mInit?.Invoke(this);
        }
        /// <summary>
        /// This method stops the service.
        /// </summary>
        public override void Stop()
        {
            StopInternal();

            PipelineInitialise();
        }

        private void StopInternal()
        {
            Pipeline.Stop();
            Pipeline.Service.StatusChanged -= OnStatusChanged;
            Pipeline = null;
            Repository = null;
        }

        /// <summary>
        /// This is the link to the repository.
        /// </summary>
        public IRepositoryAsync<K, E> Repository { get; set; }

        /// <summary>
        /// This is the link to the repository.
        /// </summary>
        public bool RepositoryRedisCacheEnabled { get; set; }
    }
}
