using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method attaches a memory persistence command to the incoming pipeline.
        /// </summary>
        /// <typeparam name="C">The incoming channel type.</typeparam>
        /// <typeparam name="K">The equatable key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="cpipe">The incoming channel pipeline.</param>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer">The entity key deserializer.</param>
        /// <param name="credentials">This is the optional azure storage credentials. 
        /// If this is not supplied, the method will try and extract this from configuration using the StorageAccountName and StorageAccountAccessKey keys.</param>
        /// <param name="startupPriority">The command start-up priority.</param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionPolicy">The version policy. This is needed if you wish to support optimistic locking for updates.</param>
        /// <param name="defaultTimeout">The default timeout. This is used for testing to simulate timeouts.</param>
        /// <param name="persistenceRetryPolicy">The retry policy. This is used for testing purposes.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="referenceMaker">The reference maker. This is used for entities that support read by reference.</param>
        /// <param name="keySerializer">The key serializer function.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPersistenceManagerDocumentDbSdk<C, K, E>(this C cpipe
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , DocumentDbConnection connection = null
            , string database = null
            , int startupPriority = 100
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<K, string> keySerializer = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            cpipe.Pipeline.AddPersistenceManagerDocumentDbSdk(keyMaker, keyDeserializer, cpipe, connection, database, startupPriority
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , referenceMaker: referenceMaker
                  , keySerializer: keySerializer);

            return cpipe;
        }
    }
}
