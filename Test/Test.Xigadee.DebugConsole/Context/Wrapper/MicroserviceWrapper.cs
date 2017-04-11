using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This wrapper is used to contain the Microservice within the console application.
    /// </summary>
    public class MicroserviceWrapper : WrapperBase
    {
        private MicroservicePipeline mPipeline;
        private Action<MicroservicePipeline, bool> mConfigure = null;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="name">The service name.</param>
        /// <param name="configure">This is the link to configuration pipeline for the Microservice</param>
        public MicroserviceWrapper(string name = null, Action<MicroservicePipeline, bool> configure = null)
        {
            Name = name;
            mConfigure = configure;
        }
        /// <summary>
        /// The current Microservice status.
        /// </summary>
        public override ServiceStatus Status
        {
            get { return mPipeline.Service.Status; }
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

            mConfigure?.Invoke(mPipeline, RedisCacheEnabled);

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
            mPipeline = null;
        }

        /// <summary>
        /// This property specifies whether Redis Cache is enabled.
        /// </summary>
        public bool RedisCacheEnabled { get; set; }

    }
}
