using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This wrapper is used to host an Api persistence client.
    /// </summary>
    /// <typeparam name="K">The entity key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class ApiPersistenceConnector<K,E> : WrapperBase<K,E>
        where K: IEquatable<K>
    {

        private ApiProviderAsyncV2<K, E> mClient;
        private ServiceStatus mStatus = ServiceStatus.Created;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="configure">This is the link to configuration pipeline for the Microservice</param>
        public ApiPersistenceConnector(Action<ApiPersistenceConnector<K, E>> configure)
        {
            Name = "API";

            if (configure == null)
                throw new ArgumentNullException("configure");

            //mClient = new ApiProviderAsyncV2<K, E>(
        }

        public override ServiceStatus Status { get { return mStatus; } }

        public override string Name { get; protected set; }

        public override void Start()
        {
            //throw new NotImplementedException();
        }

        public override void Stop()
        {
            //throw new NotImplementedException();
        }
    }
}
