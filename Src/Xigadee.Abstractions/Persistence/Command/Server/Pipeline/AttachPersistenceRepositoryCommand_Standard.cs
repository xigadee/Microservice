using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension method attaches a memory persistence command to the incoming pipeline.
        /// </summary>
        /// <typeparam name="C">The incoming channel type.</typeparam>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="repo">The repository.</param>
        /// <param name="startupPriority">The command start-up priority.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPersistenceRepositoryCommand<C>(this C cpipe
            , RepositoryBase repo
            , int startupPriority = 100
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            if (cpipe == null)
                throw new ArgumentNullException("cpipe", $"cpipe cannot be null in {nameof(AddPersistenceRepositoryCommand)}");



            return cpipe;
        }
    }
}
