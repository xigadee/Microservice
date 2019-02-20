namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This is a helper method that identifies the current pipeline. It is useful for autocomplete identification. 
        /// This command does not do anything.
        /// </summary>
        /// <param name="pipe">The pipeline.</param>
        /// <returns>The pipeline.</returns>
        public static void Ability_Is_ChannelPipelineIncoming<C>(this C pipe)
            where C: IPipelineChannelIncoming
        {
        }
        /// <summary>
        /// This is a helper method that identifies the current pipeline. It is useful for autocomplete identification. 
        /// This command does not do anything.
        /// </summary>
        /// <param name="pipe">The pipeline.</param>
        /// <returns>The pipeline.</returns>
        public static void Ability_Is_ChannelPipelineOutgoing<C>(this C pipe)
             where C : IPipelineChannelOutgoing
        {
        }
    }
}
