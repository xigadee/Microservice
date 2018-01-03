namespace Xigadee
{
    public static partial class ConsolePipelineExtensions
    {
        /// <summary>
        /// This method adds an override setting and clears the cache.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="title">The alternate title.</param>
        /// <param name="args">This is the command arguments. I will be used to parse out the shortcut parameter.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P StartWithConsole<P>(this P pipeline, string title = null, string[] args = null)
            where P : IPipeline
        {
            var items = args?.CommandArgsParse();

            var ms = pipeline.ToMicroservice();

            var mainMenu = new ConsoleMenu(title ?? ms.Id.Description ?? "Xigadee Microservice Test Console");

            mainMenu.AddMicroservicePipeline(pipeline);

            var shortcut = (items?.ContainsKey("shortcut")??false)?items["shortcut"]:null;

            mainMenu.Show(shortcut: shortcut);

            return pipeline;
        }
    }
}
