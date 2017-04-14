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
        private MicroservicePipeline mPipeline;
        private Func<MicroservicePipeline, IRepositoryAsync<K, E>> mConfigure;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="name">The service name.</param>
        /// <param name="configure">This is the link to configuration pipeline for the Microservice</param>
        public MicroservicePersistenceWrapper(string name, Func<MicroservicePipeline, IRepositoryAsync<K, E>> configure)
        {
            Name = name;
            mConfigure = configure;
        }
        /// <summary>
        /// The current Microservice status.
        /// </summary>
        public override ServiceStatus Status
        {
            get { return mPipeline?.Service?.Status ?? ServiceStatus.Created; }
        }
        /// <summary>
        /// The service name.
        /// </summary>
        public override string Name { get; protected set; }

        /// <summary>
        /// This method starts the service.
        /// </summary>
        public override void Start()
        {
            mPipeline = new MicroservicePipeline(Name);

            Repository = mConfigure?.Invoke(mPipeline);

            mPipeline.Service.StatusChanged += OnStatusChanged;
            mPipeline.Start();
        }
        /// <summary>
        /// This method stops the service.
        /// </summary>
        public override void Stop()
        {
            mPipeline.Stop();
            mPipeline.Service.StatusChanged -= OnStatusChanged;
            Repository = null;
            mPipeline = null;
        }

        /// <summary>
        /// This is the link to the repository.
        /// </summary>
        public IRepositoryAsync<K, E> Repository { get; set; }
    }
}
