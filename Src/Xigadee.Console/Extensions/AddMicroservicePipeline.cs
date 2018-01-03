using System;

namespace Xigadee
{
    public static partial class ConsoleExtensionMethods
    {
        /// <summary>
        /// This method can be called by an external process to update the info messages displayed in the menu.
        /// </summary>
        /// <param name="menu">The info message</param>
        /// <param name="pipeline">The refresh option flag.</param>
        /// <param name="useParentContextInfo">The log type.</param>
        public static ConsoleMenu AddMicroservicePipeline(this ConsoleMenu menu, IPipeline pipeline
            , bool useParentContextInfo = true)
        {
            var ms = pipeline.ToMicroservice();

            string title = $"Microservice: {ms.Id.Name}";

            var msMenu = new ConsoleMenu(title) { ContextInfoInherit = useParentContextInfo };

            menu.OnClose += (m,e) =>
            {
                if (ms.Status == ServiceStatus.Running)
                    pipeline.Stop();
            };

            msMenu.AddOption("Start", (m, o) =>
            {
                try
                {
                    pipeline.Start();
                }
                catch (Exception ex)
                {
                    msMenu.AddInfoMessage($"{ms.Id.Name} start error: {ex.Message}", true, LoggingLevel.Error);
                }
            }
            , enabled:(m,o) => ms.Status != ServiceStatus.Running
            , shortcut: ms.Id.Name
            );

            msMenu.AddOption("Stop", (m, o) => pipeline.Stop(), enabled: (m, o) => ms.Status == ServiceStatus.Running);

            //Add an option to the main menu.
            var mainMenu = menu.AddOption(new ConsoleOption(title, msMenu));

            ms.StatusChanged += (s, e) =>
            {
                mainMenu.AddInfoMessage($"{ms.Id.Name} service status changed: {e.StatusNew}", true);
            };

            return menu;
        }
    }
}
