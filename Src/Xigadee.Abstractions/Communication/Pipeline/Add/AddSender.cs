using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        public static P AddSender<P>(this P pipeline, ISender sender)
            where P : IPipeline
        {
            pipeline.Service.Communication.RegisterSender(sender);

            return pipeline;
        }


        public static P AddSender<P,S>(this P pipeline
            , Func<IEnvironmentConfiguration, S> creator = null
            , Action<S> action = null)
            where P : IPipeline
            where S : ISender, new()
        {
            var sender = creator == null ? new S() : creator(pipeline.Configuration);

            action?.Invoke(sender);

            pipeline.AddSender(sender);

            return pipeline;
        }
    }
}
