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
        /// <param name="directive">The directive.</param>
        /// <param name="repo">The optional repository. This does not need to be set if the directive has a reference to the repository. </param>
        /// <param name="startupPriority">The command start-up priority.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachPersistenceRepositoryDirective<C>(this C cpipe
            , RepositoryDirective directive
            , Func<RepositoryDirective, RepositoryBase> fnRepo = null
            , int startupPriority = 100
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            if (cpipe == null)
                throw new ArgumentNullException("cpipe", $"cpipe cannot be null in {nameof(AddPersistenceRepositoryCommand)}");

            if (directive == null)
                throw new ArgumentNullException("directive", $"directive cannot be null.");

            var repo = fnRepo?.Invoke(directive) ?? directive.Get as RepositoryBase;
            if (repo == null)
                throw new ArgumentNullException("repo", $"repo cannot be resolved from the function or the directive.");

            //OK, we need to create a persistence client.
            var client = directive.RepositoryCreatePersistenceClient();

            var server = directive.RepositoryCreatePersistenceServer(repo);

            //Attach the server to receive requests.
            cpipe.AttachCommand(server, startupPriority);

            //Attach the client to send the requests.
            cpipe.AttachCommand(client, startupPriority);

            directive.Set(client);

            return cpipe;
        }
    }
}
