using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension method attaches a memory persistence command to the incoming pipeline.
        /// </summary>
        /// <typeparam name="P">The incoming channel type.</typeparam>
        /// <typeparam name="K">The equatable key type.</typeparam>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="repo">The repository.</param>
        /// <param name="pm">An output parameter for the persistence manager.</param>
        /// <param name="startupPriority">The command start-up priority.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPersistenceRepositoryCommand<C, K, E>(this C cpipe
            , IRepositoryAsyncServer<K, E> repo
            , out RepositoryPersistenceWrapperCommand<K, E> pm
            , int startupPriority = 100
            )
            where C : IPipelineChannelIncoming<IPipeline>
            where K : IEquatable<K>
        {
            //if (keyMaker == null)
            //    throw new ArgumentNullException("keyMaker", $"keyMaker cannot be null in {nameof(AddPersistenceManagerMemory)}");
            //if (keyDeserializer == null)
            //    throw new ArgumentNullException("keyDeserializer", $"keyDeserializer cannot be null in {nameof(AddPersistenceManagerMemory)}");
            //if (cpipe == null)
            //    throw new ArgumentNullException("cpipe", $"cpipe cannot be null in {nameof(AddPersistenceRepositoryCommand)}");

            pm = null;

            //keyMaker, keyDeserializer
            //  , entityName: entityName
            //  , versionPolicy: versionPolicy
            //  , defaultTimeout: defaultTimeout
            //  , persistenceRetryPolicy: persistenceRetryPolicy
            //  , resourceProfile: resourceProfile
            //  , cacheManager: cacheManager
            //  , referenceMaker: referenceMaker
            //  , referenceHashMaker: referenceHashMaker
            //  , keySerializer: keySerializer
            //  , prePopulate: prePopulate);

            cpipe.Pipeline.AddPersistenceRepositoryCommand(repo, cpipe, out pm, startupPriority);

            return cpipe;
        }
    }
}
