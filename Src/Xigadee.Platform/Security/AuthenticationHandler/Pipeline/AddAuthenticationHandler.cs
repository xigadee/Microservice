using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method adds the encryption handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier. 
        /// This is will be used when assigning the handler to a channel or collector.</param>
        /// <param name="handler">The handler instance.</param>
        /// <param name="action">The action on the handler.</param>
        /// <returns>The pipeline.</returns>
        public static P AddAuthenticationHandler<P>(this P pipeline
            , string identifier
            , IAuthenticationHandler handler
            , Action<IAuthenticationHandler> action = null)
            where P : IPipeline
        {
            action?.Invoke(handler);

            pipeline.Service.Security.RegisterAuthenticationHandler(identifier, handler);

            return pipeline;
        }

        /// <summary>
        /// This method adds the encryption handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier. 
        /// <param name="creator">This function is used to create the handler from the configuration collection.</param>
        /// <param name="action">The action on the handler.</param>
        /// <returns>The pipeline.</returns>
        public static P AddAuthenticationHandler<P>(this P pipeline
            , string identifier
            , Func<IEnvironmentConfiguration, IAuthenticationHandler> creator
            , Action<IAuthenticationHandler> action = null)
            where P : IPipeline
        {
            var handler = creator(pipeline.Configuration);

            action?.Invoke(handler);

            pipeline.Service.Security.RegisterAuthenticationHandler(identifier, handler);

            return pipeline;
        }
    }
}
