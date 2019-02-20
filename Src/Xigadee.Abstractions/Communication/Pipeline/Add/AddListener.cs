using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension method adds a listener to the collection.
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="pipeline"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static P AddListener<P>(this P pipeline, IListener listener)
            where P : IPipeline
        {
            pipeline.Service.Communication.RegisterListener(listener);

            return pipeline;
        }

        public static P AddListener<P,S>(this P pipeline
            , Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where P : IPipeline
            where S : IListener
        {
            var listener = creator(pipeline.Configuration);

            action?.Invoke(listener);

            pipeline.AddListener(listener);

            return pipeline;
        }
    }
}
