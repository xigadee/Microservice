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

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="name">The service name.</param>
        /// <param name="configure">This is the link to configuration pipeline for the Microservice</param>
        public MicroservicePersistenceWrapper(string name, Action<MicroservicePersistenceWrapper<K,E>> configure)
        {
            Name = name;
            mConfigure = configure;
        }
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
            Pipeline = new MicroservicePipeline(Name);

            mConfigure?.Invoke(this);

            Pipeline.Service.StatusChanged += OnStatusChanged;

            Pipeline.Start();
        }
        /// <summary>
        /// This method stops the service.
        /// </summary>
        public override void Stop()
        {
            Pipeline.Stop();
            Pipeline.Service.StatusChanged -= OnStatusChanged;
            Repository = null;
            Pipeline = null;
        }

        /// <summary>
        /// This is the link to the repository.
        /// </summary>
        public IRepositoryAsync<K, E> Repository { get; set; }
    }
}
