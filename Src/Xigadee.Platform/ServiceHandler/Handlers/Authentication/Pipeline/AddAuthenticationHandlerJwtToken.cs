using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method adds the JWT authentication handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier. 
        /// This is will be used when assigning the handler to a channel or collector.</param>
        /// <param name="algo">The HMAC algorithm.</param>
        /// <param name="base64Secret">This is the base64 encoded byte array used for the HMAC secret.</param>
        /// <param name="action">The action on the handler.</param>
        /// <returns>The pipeline.</returns>
        public static P AddJwtTokenAuthenticationHandler<P>(this P pipeline
            , string identifier
            , JwtHashAlgorithm algo
            , string base64Secret
            , Action<IServiceHandlerAuthentication> action = null)
            where P : IPipeline
        {
            if (algo == JwtHashAlgorithm.None)
                throw new ArgumentOutOfRangeException("JwtHashAlgorithm.None is not supported.");

            if (string.IsNullOrEmpty(base64Secret))
                throw new ArgumentNullException($"base64Secret cannot be null or empty for {nameof(AddJwtTokenAuthenticationHandler)}");

            var handler = new JwtTokenAuthenticationHandler(identifier, algo, base64Secret);

            pipeline.AddAuthenticationHandler(handler, action);

            return pipeline;
        }

        /// <summary>
        /// This method adds the JWT authentication handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier. 
        /// This is will be used when assigning the handler to a channel or collector.</param>
        /// <param name="algo">The HMAC algorithm.</param>
        /// <param name="secretSetter">This is the function to return the base64 encoded secret from configuration.</param>
        /// <param name="action">The action on the handler.</param>
        /// <returns>The pipeline.</returns>
        public static P AddJwtTokenAuthenticationHandler<P>(this P pipeline
            , string identifier
            , JwtHashAlgorithm algo
            , Func<IEnvironmentConfiguration, string> secretSetter
            , Action<IServiceHandlerAuthentication> action = null)
            where P : IPipeline
        {
            if (algo == JwtHashAlgorithm.None)
                throw new ArgumentOutOfRangeException("JwtHashAlgorithm.None is not supported.");

            if (secretSetter == null)
                throw new ArgumentNullException($"secretSetter cannot be null or empty for {nameof(AddJwtTokenAuthenticationHandler)}");

            byte[] bySecret = null;
            try
            {
                bySecret = Convert.FromBase64String(secretSetter(pipeline.Configuration));
            }
            catch (Exception ex)
            {
                throw new AuthenticationHandlerInvalidSecretException(ex);
            }

            var handler = new JwtTokenAuthenticationHandler(identifier, algo, bySecret);

            pipeline.AddAuthenticationHandler(handler, action);

            return pipeline;
        }

        /// <summary>
        /// This method adds the JWT authentication handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier. 
        /// This is will be used when assigning the handler to a channel or collector.</param>
        /// <param name="algo">The HMAC algorithm.</param>
        /// <param name="secret">This is the byte array used for the HMAC secret.</param>
        /// <param name="audience">This is the audience value generated for the token and used for validation when a token is received.</param>
        /// <param name="action">The action on the handler.</param>
        /// <returns>The pipeline.</returns>
        public static P AddAuthenticationHandlerJwtToken<P>(this P pipeline
            , string identifier
            , JwtHashAlgorithm algo
            , byte[] secret
            , string audience = "comms"
            , Action<IServiceHandlerAuthentication> action = null)
            where P : IPipeline
        {
            if (algo == JwtHashAlgorithm.None)
                throw new ArgumentOutOfRangeException("JwtHashAlgorithm.None is not supported.");

            if (secret == null || secret.Length == 0)
                throw new ArgumentNullException($"secret cannot be null or empty for {nameof(AddJwtTokenAuthenticationHandler)}");

            var handler = new JwtTokenAuthenticationHandler(identifier, algo, secret, audience: audience);

            pipeline.AddAuthenticationHandler(handler, action);

            return pipeline;
        }
    }
}
